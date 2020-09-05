using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManger : MonoBehaviour
{
    public string PlaneName;
    public double MaxHP;//最大血量
    public int Level=0;//当前等级
    public double Damage;//基础伤害
    public double Speed;//移速
    private double TurnAngle;//转向系数
    private double AttackCD;//飞机当前的攻击CD
    public double MaxEnergy;//飞船能量当飞船加速或者发射子弹时消耗能量
    private double EnergyRecover;//能量恢复速度
    private double ShootConsume;//射击消耗能量,每发子弹
    private double SpeedUpConsume;//加速消耗的能量
    private AmmoType ammoType;//飞船的弹药类型
    private double bulletSpeed;//子弹飞行速度
    private float HealHP;
    //玩家当前属性
    public double MineExp;//飞机身上的矿物数量相当于当前经验
    public double NowEnergy;//当前能量
    public double NowHP;//血量
    private double NowScale;//当前缩放
    private double ShootDistance;//攻击范围
    //玩家升级方向属性
    public float volume = 1;//音量
    private GameObject[] shootPos;//飞船开火的位置
    private GameObject[] Tails;//飞船拖尾效果的摆放位置
    public GameObject[] bulletPrefabs;//飞船的子弹
    private Transform Pos1;
    private Transform Pos2;
    public AudioClip shoot;
    public GameObject DeathEft;
    private bool hasEft = false;
    private float Timer;
    private Vector3 dir;//开火和移动的方向
    private Vector2 planeDir;
    private bool isOutOfBattle;//脱战
    private float OutOfBattleTimer;//脱战计时器
    private float HealTimer;
    private GameObject[] Tail_Normal;
    private GameObject[] Tail_Speed;
    private AudioSource audioSource;
    private Rigidbody PlayerRig;
    private bool Loaded = false;
    public GameObject[] TailEffects;//0.引擎效果 1.引擎加速效果 
    private float shootCD;
    private Vector2 V2 = new Vector2();
    private Vector3 Ore2Enemy;
    public GameObject PickOreEft;
    //敌人AI变量
    public EnemyState enemyState = EnemyState.Idle;
    public GameObject Target = null;
    public Vector3 TargetPos = new Vector3();
    private bool PlayerInTrigger = false;

    private float FollowTimer=0f;
    private float RandomTimer = 0f;
    public bool AllowMove = false;
    private float TargetChooseTimer;
    public AudioClip DeathSound;
    public UpdateBars updateBars;
    private bool IsLoadComplete = false;


    //敌人强度设置属性
    private float MoveTowardAngle=20;
    private float ShootFollowPlayerTimer=0.5f;
    private float ShootViewAngle = 30f;
    //敌人的攻击欲望
    private float SpeedUpDistance = 30;
    private float FindPlayerTimer=7f;
    private void Awake()
    {
        shootPos = SupputFuction.FindGameObjectsWithTagInChild(this.transform, "Weapon");
        Tails = SupputFuction.FindGameObjectsWithTagInChild(this.transform, InputString_Tags.tailTag);
        Pos1 = SupputFuction.FindGameObjectWithTagInChild(this.transform, InputString_Tags.Dir).GetComponent<Transform>();
        Pos2 = SupputFuction.FindGameObjectWithTagInChild(this.transform, InputString_Tags.Dir1).GetComponent<Transform>();
        Debug.Log("敌人飞船武器为：" + shootPos.Length + "引擎数量为" + Tails.Length);
        audioSource = GetComponent<AudioSource>();
        dir = Pos2.position - Pos1.position;
        dir = new Vector3(dir.x, dir.y, 0);
        PlayerRig = GetComponent<Rigidbody>();
        Loaded = true;

    }
    // Start is called before the first frame update
    void Start()
    {
        V2 = new Vector2(Random.Range(-10f, 10F), Random.Range(-10, 10F)).normalized;
        LoadJsonData();
        StartCoroutine(CreatTail());
    }
    // Update is called once per frame
    void Update()
    {
        dir = Pos2.position - Pos1.position;
        planeDir = new Vector2(dir.x, dir.y).normalized;
        //print(dir + "dir" + planeDir);
        HealHP_Do();
        shootCD += Time.deltaTime;
        if (!isOutOfBattle)
        {
            OutOfBattleTimer += Time.deltaTime;
            if (OutOfBattleTimer > 4f)
            {
                isOutOfBattle = true;
                OutOfBattleTimer = 0;
            }
        }
        EnemyAI();
    }
    public IEnumerator CreatTail()
    {
        if (Tails == null)
            yield return null;
        if (!Loaded)
            yield return null;
        Tail_Normal = new GameObject[Tails.Length];
        Tail_Speed = new GameObject[Tails.Length];
        audioSource.volume = volume * 0.3f;
        for (int i = 0; i < Tails.Length; i++)//开始游戏时给加上拖尾，并隐藏掉
        {
            Tail_Normal[i] = Instantiate(TailEffects[0], Tails[i].transform, false);
            Tail_Normal[i].transform.position = Tails[i].transform.position;
            Tail_Normal[i].SetActive(false);
            Tail_Speed[i] = Instantiate(TailEffects[1], Tails[i].transform, false);
            Tail_Speed[i].transform.position = Tails[i].transform.position;
            Tail_Speed[i].SetActive(false);

        }
    }
    private void LoadJsonData()//把json数据读取出来赋值给当前目标
    {
        foreach (var p in JsonLoad.instance.planes)
        {
            if (p.PlaneName == PlaneName)
            {
                
                NowHP = MaxHP = p.HP*1.4;
                Level = 1;
                Damage = p.Damage;
                Speed = p.Speed;
                TurnAngle = p.TurnAngle;
                AttackCD = p.AttackCD*2;
                NowEnergy = MaxEnergy = p.Energy;
                EnergyRecover = p.EnergyRecover;
                ShootConsume = p.ShootConsume;
                SpeedUpConsume = p.SpeedUpConsume;
                ShootDistance = 20f;
                bulletSpeed = p.bulletSpeed;
                switch (p.ammoType)
                {
                    case 0:
                        ammoType = AmmoType.bullet;
                        break;
                    case 1:
                        ammoType = AmmoType.BigBall;
                        break;
                    case 2:
                        ammoType = AmmoType.Ray;
                        break;
                }
                print("从Json读取数据成功！");
                IsLoadComplete = true;
            }
        }
    }
    private void OnTriggerEnter(Collider other)//当玩家在敌人视野内向玩家移动
    {
        if (other.tag == InputString_Tags.PlayerTag)
        {
            Target = other.gameObject;
            enemyState = EnemyState.Attack;
            PlayerInTrigger = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == InputString_Tags.PlayerTag)
        {
            PlayerInTrigger = false;
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag(InputString_Tags.OreTag))
        {
            Ore2Enemy = transform.position - other.transform.position;
            other.transform.Translate(Ore2Enemy * Time.deltaTime, Space.World);
            if (Vector3.Distance(other.transform.position, transform.position) < 5f)
            {
                Ore ore = other.gameObject.GetComponent<Ore>();
                MineExp += ore.ResourceNum;
                HpChange(ore.HealHp);
                EngeryChange(ore.HealEngery);
                Destroy(other.gameObject);
                if (!hasEft)
                {
                    SupputFuction.EftPlayInParent(PickOreEft, transform);//播放特效
                    hasEft = true;
                }
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != InputString_Tags.PlayerTag)
        {
            V2 = -V2;

        }
    }
    public void TakeDamage(float damage)
    {
        NowHP -= damage ;
        isOutOfBattle = false;
        OutOfBattleTimer = 0;
        print("敌人飞船受到伤害为：" + damage );
        if (NowHP < 0)
            Death();
        updateBars.UpdateUI();
        if (PlayerManager.instance != null)
        {
            enemyState = EnemyState.Attack;
            TargetPos = PlayerManager.instance.transform.position;
            Target = PlayerManager.instance.gameObject;
        }
    }
    public void HpChange(float num)//血量变更方法
    {
        NowHP += num;
        if (NowHP > MaxHP)
            NowHP = MaxHP;
    }
    public void EngeryChange(float num)//能量变更方法
    {
        NowEnergy += num;
        if (NowEnergy > MaxEnergy)
            NowEnergy = MaxEnergy;
    }
    void Death()//敌人死亡方法
    {
        SupputFuction.EftPlayInParent(GameManger.Instance.DeathEft, transform);
        enemyState = EnemyState.death;
        GameManger.Instance.enemyList.Remove(gameObject);
        SupputFuction.CreatOre((int)MineExp / 15, transform.position, 10);
        SupputFuction.PlayAudio(DeathSound, transform.position);
        UI_Manger.instance.UpdatePointText(1, Level * 100);
        Destroy(updateBars.gameObject);
        Destroy(gameObject);
    }
    public void LevelUp()
    {
        //等级增加
        Level++;
        print("敌人的levelUp执行了！"+Level);
        //升级属性参数
        MineExp = 0;
        MaxHP *= 1.15;
        MaxEnergy *= 1.1;
        Damage *= 1.1;
        Speed *= 0.97;
        TurnAngle *= 1.04;
        AttackCD *= 0.94f;
        NowScale *= 1.03;
        ShootDistance *= 1.03;
        //恢复血量和能量
        HpChange((float)MaxHP * 0.4f);
        EngeryChange((float)(MaxEnergy * 0.6f));
        if (Level == 6 || Level == 12)
        {
            HealHP += 2;
        }
    }
    void HealHP_Do()
    {
        if (HealTimer > 0.5)
        {
            if (isOutOfBattle)
            {
                HpChange((float)HealHP * 2f);
                EngeryChange((float)EnergyRecover);
                HealTimer = 0;
            }
            else
            {
                HpChange((float)HealHP);
                EngeryChange((float)EnergyRecover/2);
                HealTimer = 0;
            }
        }
        else
        {
            HealTimer += Time.deltaTime;
        }
    }
    void EnemyMove(Vector2 moveDir, bool SpeedUp=false)
    {
        if (!AllowMove)
            return;
       // print("敌人运动夹角" + Vector2.SignedAngle(planeDir, moveDir) + "moveDir"+moveDir+"planeDir"+planeDir);
        if (Vector2.SignedAngle(planeDir, moveDir) > MoveTowardAngle)
        {
            transform.Rotate(0, -Mathf.Abs((float)TurnAngle * Time.deltaTime) / 4, 0);
        }
        else if (Vector2.SignedAngle(planeDir, moveDir) < -MoveTowardAngle)
        {
            transform.Rotate(0, Mathf.Abs((float)TurnAngle * Time.deltaTime) / 4, 0);
        }

        if (moveDir.x == 0 && moveDir.y == 0)
        {
            for (int z = 0; z < Tails.Length; z++)
            {
                Tail_Normal[z].SetActive(false);
            }

        }
        else
        {
            if (Vector2.SignedAngle(planeDir, moveDir) < 50 && Vector2.SignedAngle(planeDir, moveDir) > -30)
            {
                PlayerRig.AddForce(dir * (float)Speed);
                for (int z = 0; z < Tails.Length; z++)
                {
                    Tail_Normal[z].SetActive(true);
                }
            }
        }
        if (SpeedUp && NowEnergy > 1)//加速
        {
            PlayerRig.AddForce(dir * (float)Speed *1.6f);
            NowEnergy -= Time.deltaTime * (SpeedUpConsume);
            for (int z = 0; z < Tails.Length; z++)
            {
                Tail_Normal[z].SetActive(false);
                Tail_Speed[z].SetActive(true);
            }
        }
        else
        {
            audioSource.Stop();
            for (int z = 0; z < Tails.Length; z++)
            {
                Tail_Speed[z].SetActive(false);
            }
        }
    }
    void EnemyShoot()
    {
        if (shootCD >= AttackCD && NowEnergy > ShootConsume)
        {
            foreach (var pos in shootPos)
            {
                GameObject bullet = Instantiate(bulletPrefabs[0], GameManger.Instance.BulletPool.transform, false);
                bullet.transform.localRotation = Quaternion.Euler(transform.eulerAngles);
                bullet.transform.position = pos.transform.position;
                bullet.GetComponent<EnemyBullet>().Shoot(gameObject, dir,(float)Damage);
                AudioSource.PlayClipAtPoint(shoot, pos.transform.position, PlayerManager.instance.volume);
            }
            //Debug.Log(ShootPos==null?"ShootPos为空":"正常");
            shootCD = 0;
            NowEnergy -= ShootConsume*1.56f;
        }
    }

    private void  EnemyAI()
    {
        if (!PlayerInTrigger)
        {
            if(FollowTimer > FindPlayerTimer)
            {
                Target = null;
                enemyState = EnemyState.Idle;
                FollowTimer = 0f;
            }
            else
            {
                FollowTimer += Time.deltaTime;
            }

        }
        else
        {
            FollowTimer = 0f;
        }
        if (Target != null&&TargetChooseTimer> ShootFollowPlayerTimer)
        {
            TargetPos = Target.transform.position;
            TargetChooseTimer = 0;
        }
        else
        {
            TargetChooseTimer += Time.deltaTime;
        }
        switch (enemyState)
        {
            case EnemyState.Attack:
                EnemyAttack();
                break;
            case EnemyState.Idle:
                EnemyIdle();
                break;
            case EnemyState.death:
                Death();
                break;
        }
    }
    
    private void EnemyIdle()
    {
        if (RandomTimer > 4f)
        {
            V2 = new Vector2(Random.Range(-10f, 10F), Random.Range(-10f, 10F)).normalized;
            RandomTimer = 0f;
            if ((transform.position.y > MapManger.instance.MapHeight * 0.45
               || transform.position.y < -MapManger.instance.MapHeight * 0.45)
               || (transform.position.x > MapManger.instance.MapWidth * 0.45
               || transform.position.x < -MapManger.instance.MapWidth * 0.45))//判断AI是否在地图边界
            {
                if (PlayerManager.instance != null)
                {   enemyState = EnemyState.Attack;
                    TargetPos = PlayerManager.instance.transform.position;
                    Target = PlayerManager.instance.gameObject;
                    FollowTimer = FindPlayerTimer-3;
                }
            }
        }
        else
        {
            RandomTimer += Time.deltaTime;
        }

        
        EnemyMove(V2);
    }

    private void EnemyAttack()
    {
        if (Target != null)
        {
            Vector2 v = new Vector2(-transform.position.x + TargetPos.x, -transform.position.y + TargetPos.y).normalized;
            if (Vector3.Distance(transform.position, TargetPos) > SpeedUpDistance)
            {
                EnemyMove(v,true);
            }
            else
            {
                EnemyMove(v, false);
            }

            if(((Vector3.SignedAngle(transform.position- TargetPos, dir,Vector3.forward)>180- ShootViewAngle)||
               (Vector3.SignedAngle(transform.position - TargetPos, dir, Vector3.forward) < ShootViewAngle-180))&& Vector3.Distance(transform.position, TargetPos) <20)
            {
                EnemyShoot();
            }
        }
    }


    public void CreateEnemy(int level,int res)
    {
        StartCoroutine(Creat(level, res));
    }

    private IEnumerator Creat(int level,int res)
    {
        if (!IsLoadComplete)
            yield return null;
        for (int i = 0; i < level; i++)
        {
            LevelUp();
            print("敌人升级了" + i);
        }
        MineExp = res;

        switch (PlayerPrefs.GetInt("Enemy")){
            case 0:
                MoveTowardAngle *= 1.2f;
                ShootFollowPlayerTimer *= 1.5f;
                ShootViewAngle *= 1.5f;
                SpeedUpDistance *= 1.25f;
                FindPlayerTimer = 4f;
                NowHP *= 0.6;
                MaxHP *= 0.6;
                Damage *= 0.4;
                print("当前敌人难度为简单");
                break;
            case 1:
                MoveTowardAngle *= 1f;
                ShootFollowPlayerTimer *= 1.2f;
                ShootViewAngle *= 1f;
                SpeedUpDistance *= 1f;
                FindPlayerTimer = 5f;
                NowHP *= 0.8;
                MaxHP *= 0.8;
                Damage *= 0.5;
                print("当前敌人难度为中等");
                break;
            case 2:
                MoveTowardAngle *= 0.8f;
                ShootFollowPlayerTimer *= 0.6f;
                ShootViewAngle *= 0.9f;
                SpeedUpDistance *= 0.8f;
                FindPlayerTimer = 6f;
                NowHP *= 1;
                MaxHP *= 1;
                Damage *= 0.7;
                print("当前敌人难度为困难");
                break;
            case 3:
                MoveTowardAngle *= 0.8f;
                ShootFollowPlayerTimer *= 0.3f;
                ShootViewAngle *= 0.7f;
                SpeedUpDistance *= 0.7f;
                FindPlayerTimer = 6f;
                NowHP *= 1;
                MaxHP *= 1;
                Damage *= 0.8;
                print("当前敌人难度为疯狂");
                break;
            case 4:
                MoveTowardAngle *= 0.6f;
                ShootFollowPlayerTimer *= 0.4f;
                ShootViewAngle *= 0.6f;
                SpeedUpDistance *= 0.6f;
                FindPlayerTimer= 8f;
                NowHP *= 1;
                MaxHP *= 1;
                Damage *= 1;
                print("当前敌人难度为地狱");
                break;
        }
    }
}

public enum EnemyState
{
    Idle,
    Attack,
    death
}