using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;
    //玩家属性
    public string PlaneName;
    public double MaxHP;//最大血量
    public int Level;//当前等级
    public double Exp2NextLevel;//下一等级需要的经验
    public double Damage;//基础伤害
    public double Speed;//移速
    public double TurnAngle;//转向系数
    public double AttackCD;//飞机当前的攻击CD
    public double MaxEnergy;//飞船能量当飞船加速或者发射子弹时消耗能量
    public double EnergyRecover;//能量恢复速度
    public double ShootConsume;//射击消耗能量,每发子弹
    public double SpeedUpConsume;//加速消耗的能量
    public AmmoType ammoType;//飞船的弹药类型
    public double bulletSpeed;//子弹飞行速度


    //玩家当前属性
    public double MineExp;//飞机身上的矿物数量相当于当前经验
    public double NowEnergy;//当前能量
    public double NowHP;//血量
    public double NowScale;//当前缩放
    public double View;//当前视野
    public double ShootDistance;//攻击范围
    //玩家升级方向属性
    public double DamageReduce;//伤害降低，最大为-35%      
    public double TurnAngleEnhance;//操作系数加强          
    public double SpeedEnhance;//操作系数加强              
    public double AttackSpeedEnhance;//加强攻速
    public double HP_Enhance;//血量加强
    public double DamageEnhance;//伤害加强
    public double healHP;//恢复血量
    public double EnergyRecoverEnhance;//回复能量的速度
    public double EnergyEnhance;//能量最大值增加
    public double Damage2Mine;//对矿石伤害增加倍数
    public double equipDamage;//道具伤害系数
    public double equipNum;//道具数量增强
    public double PickOreRange;//拾取矿石的范围，默认为7
    public double ScaleEnhance;//飞机的大小
    public double ShootDistanceEnhance;//攻击范围增强
    public double BulletSpeedenhance;//子弹移速增强
    //实现json读取飞船数据
   

    public float volume=1;//音量


    public GameObject[] shootPos;//飞船开火的位置
    public GameObject[] Tails;//飞船拖尾效果的摆放位置
    public GameObject[] bulletPrefabs;//飞船的子弹
    private Transform Pos1;
    private Transform Pos2;
    public SphereCollider Trigger;
    public AudioClip PickUpAudio;
    public GameObject PickOreEft;
    public GameObject LevelUpEft;


    private Vector3 Ore2Player;
    public float OreMove2PlayerSpeed = 1f;
    private bool hasEft = false;
    private float Timer;
    public Vector3 dir;//开火和移动的方向
    private bool playLevelUpEFT = false;
    private float playLevelUpEFTTimer;

    private bool isOutOfBattle=true;//脱战
    private float OutOfBattleTimer;//脱战计时器
    private float HealTimer;

    public bool Loaded = false;
    private void Awake()
    {
        instance = this;
        shootPos=SupputFuction.FindGameObjectsWithTagInChild(this.transform,"Weapon");
        Tails = SupputFuction.FindGameObjectsWithTagInChild(this.transform, InputString_Tags.tailTag);
        Pos1= SupputFuction.FindGameObjectWithTagInChild(this.transform, InputString_Tags.Dir).GetComponent<Transform>();
        Pos2= SupputFuction.FindGameObjectWithTagInChild(this.transform, InputString_Tags.Dir1).GetComponent<Transform>();
        Debug.Log("飞船武器为："+shootPos.Length+"引擎数量为"+Tails.Length);
        dir = Pos2.position - Pos1.position;
        dir = new Vector3(dir.x, dir.y, 0);
        Loaded = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        LoadJsonData();

        equipNum = 1;
        Trigger = GetComponent<SphereCollider>();
        Trigger.radius = 10;
        Camera.main.GetComponent<CasmerFollowPlayer>().playerPos = transform;
    }
    // Update is called once per frame
    void Update()
    {
        dir = Pos2.position - Pos1.position;
        dir = new Vector3(dir.x, dir.y,0 );

        if (Timer<0)
        {
            hasEft = false;
            Timer = 2f;
        }
        else
        {
            if (hasEft)
            {
                Timer -= Time.deltaTime;
            }
        }
        if (playLevelUpEFT)
        {
            playLevelUpEFTTimer += Time.deltaTime;
            if (playLevelUpEFTTimer > 1.5f)
            {
                playLevelUpEFT = false;
                playLevelUpEFTTimer = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.F10))
            LevelUp();

        HealHP_Do();


        if (!isOutOfBattle)
        {
            OutOfBattleTimer += Time.deltaTime;
            if (OutOfBattleTimer > 4f)
            {
                isOutOfBattle = true;
                OutOfBattleTimer = 0;
            }
        }

    }


    private void LoadJsonData()//把json数据读取出来赋值给当前目标
    {
        foreach (var p in JsonLoad.instance.planes)
        {
            if (p.PlaneName == PlaneName)
            {
                NowHP=MaxHP = p.HP;
                Level = 1;
                Exp2NextLevel = p.Exp2NextLevel;
                Damage = p.Damage;
                Speed = p.Speed;
                TurnAngle = p.TurnAngle;
                AttackCD = p.AttackCD;
                NowEnergy=MaxEnergy = p.Energy;
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
            }
        }
    }

    private void OnTriggerStay(Collider other)//当目标在触发器内使目标向自己移动
    {
        if (other.gameObject.CompareTag(InputString_Tags.OreTag))
        {
            Ore2Player = transform.position - other.transform.position;
            other.transform.Translate(Ore2Player * OreMove2PlayerSpeed * Time.deltaTime,Space.World);
            if (Vector3.Distance(other.transform.position, transform.position) < 5f)
            {
                Ore ore = other.gameObject.GetComponent<Ore>();

                if (MineExp + ore.ResourceNum > Exp2NextLevel)
                {
                    LevelUp();
                }
                else
                {
                    MineExp += ore.ResourceNum;
                    UI_Manger.instance.UpdatePointText(0, (int)ore.ResourceNum);
                }
                HpChange(ore.HealHp);
                EngeryChange(ore.HealEngery);
                Destroy(other.gameObject);
                if (!hasEft)
                {
                    SupputFuction.EftPlayInParent(PickOreEft, transform);//播放特效
                    hasEft = true;
                }
                SupputFuction.PlayAudio(PickUpAudio, transform.position);//播放声音
            }
        }
    }

    public void TakeDamage(float damage)
    {
        NowHP -= damage *(1- DamageReduce);
        isOutOfBattle = false;
        OutOfBattleTimer = 0;
        print("飞船受到伤害为：" + damage * (1 - DamageReduce));
        UI_Manger.instance.UpdateUI();
        if (NowHP < 0)
            Death();

    }
   public void HpChange(float num )//血量变更方法
    {
        NowHP = num+NowHP;
        if (NowHP > MaxHP)
            NowHP = MaxHP;
    }

    public void EngeryChange(float num)//能量变更方法
    {
        NowEnergy += num;
        if (NowEnergy > MaxEnergy)
            NowEnergy = MaxEnergy;
    }
    void Death()//玩家死亡方法
    {
        SupputFuction.EftPlayInParent(GameManger.Instance.DeathEft, transform);
        GameManger.Instance.PlayerIsDeath = true;
        SupputFuction.CreatOre((int)MineExp / 15, transform.position, 6);
        MineExp = 0;

        SupputFuction.PlayAudio(GameManger.Instance.PlayerDeathAudio, transform.position);
        Destroy(gameObject);
    }

    public void LevelUp(bool Reborn=false)
    {
        if (Level >= 15)
            return;
        //等级增加
        Level++;
        //播放特效
        if (!playLevelUpEFT)
        {
            SupputFuction.EftPlayInParent(LevelUpEft, transform, true);
            playLevelUpEFT = true;
        }  
        //升级属性参数
        MineExp = 0;
        Exp2NextLevel = 1.3 * Exp2NextLevel;
        MaxHP *= 1.15 ;
        MaxEnergy *= 1.05;
        Damage *=1.05 ;
        Speed *= 0.97;
        TurnAngle *= 0.98;
        NowScale *= 1.03;
        ShootDistance *= 1.03;
        View *= 1.03;
        //恢复血量和能量
        HpChange((float)MaxHP * 0.4f);
        EngeryChange((float)(MaxEnergy * 0.6f));
        if (Level == 6 || Level == 12)
        {
            healHP += 2;
        }
        if(!Reborn)
        LoadLevelUpOption.instance.ShowLevelUpOption(true);

    }
    void HealHP_Do()
    {
        if (HealTimer > 0.4)
        {
            if (isOutOfBattle)
            {
                HpChange((float)healHP*2);
                EngeryChange((float)(EnergyRecover + EnergyRecoverEnhance));
                HealTimer = 0;
            }
            else
            {
                HpChange((float)healHP);
                EngeryChange((float)(EnergyRecover + EnergyRecoverEnhance)*0.4f);
                HealTimer = 0;
            }
        }
        else
        {
            HealTimer += Time.deltaTime;
        }
    }


    public void ChooseLevelUpOption(int id)//得到升级方向属性
    {
        LevelUpOption levelUpOption = JsonLoad.instance.FindLevelUpOptionWithID(id);       //
        AttackSpeedEnhance += levelUpOption.AttackSpeedEnhance;
            //实现了
        DamageReduce += levelUpOption.DamageReduce;                   //实现了
        TurnAngleEnhance += levelUpOption.TurnAngleEnhance;           //实现了
        SpeedEnhance += levelUpOption.SpeedEnhance;                   //实现了
        MaxHP += levelUpOption.HP_Enhance;                            //实现了
        HpChange((float) levelUpOption.HP_Enhance);
        DamageEnhance += levelUpOption.DamageEnhance;                 //实现了一半
        healHP += levelUpOption.healHP;                               // 实现了
        EnergyRecoverEnhance += levelUpOption.EnergyRecoverEnhance;   //实现了
        MaxEnergy += levelUpOption.EnergyEnhancek;                    //实现了
        EngeryChange((float)levelUpOption.EnergyEnhancek);
        Damage2Mine += levelUpOption.Damage2Mine;                     //实现了
        equipDamage += levelUpOption.equipDamage;                     // 未定义！
        equipNum += levelUpOption.equipNum;                           //  实现了一半
        PickOreRange = levelUpOption.PickOreRange;                   //
        ScaleEnhance += levelUpOption.ScaleEnhance;                   //实现了
        ShootDistanceEnhance += levelUpOption.ShootDistanceEnhance;   //实现了
        BulletSpeedenhance += levelUpOption.BulletSpeedenhance;       //  实现了

        Trigger.radius *= (float)(1 + PickOreRange);
        
    }


}



public enum AmmoType//飞机的弹药类型
{
    bullet,
    BigBall,
    Ray
}
public struct plane
{
    public string PlaneName;
    public int HP;
    public int Level;
    public double Exp2NextLevel;
    public double Damage;
    public double Speed;
    public double TurnAngle;
    public double AttackCD;
    public double Energy;
    public double EnergyRecover;
    public double ShootConsume;
    public double SpeedUpConsume;
    public int ammoType;
    public double bulletSpeed;
}


public struct LevelUpOption
{
    public string Name;
    public int ID;
    public int type;
    public double DamageReduce;//伤害降低，最大为-35%        
    public double TurnAngleEnhance;//操作系数加强          
    public double SpeedEnhance;//操作系数加强              
    public double AttackSpeedEnhance;//加强攻速
    public double HP_Enhance;//血量加强
    public double DamageEnhance;//伤害加强
    public double healHP;//恢复血量
    public double EnergyRecoverEnhance;//回复能量的速度
    public double EnergyEnhancek;//能量最大值增加
    public double Damage2Mine;//对矿石伤害增加倍数
    public double equipDamage;//道具伤害系数
    public double equipNum;//道具数量增强
    public double PickOreRange;//拾取矿石的范围，默认为7
    public double ScaleEnhance;//飞机的大小
    public double ShootDistanceEnhance;//攻击范围增强
    public double BulletSpeedenhance;//子弹移速增强
}