using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove instance;
    private float TurnAngle;
    private float Speed;
    public Rigidbody PlayerRig;
    private PlayerManager playerManager;
    private float AngleNow = 0;
    public float MaxAngle = 30;
    private Vector3 dir;
    public AudioSource audioSource;


    // public AudioClip SpeedUpAudio;
    //  public AudioClip moveAudio;
    private GameObject[] tailPos; 
    public GameObject[] TailEffects;//0.引擎效果 1.引擎加速效果 
    public GameObject[] Fire;//开火效果 

    private bool IsSpeedUp = false;
    private float Y_rotation;

    private GameObject[] Tail_Normal;
    private GameObject[] Tail_Speed;
    private bool IsRunOnWin = true;
    public void SetSpeed()
    {
        TurnAngle = (float)(playerManager.TurnAngle*(1+ playerManager.TurnAngleEnhance));
        Speed =(float)( playerManager.Speed*(1+ playerManager.SpeedEnhance));
    }

    private void Awake()
    {
        instance = this;
        playerManager = GetComponent<PlayerManager>();
        PlayerRig = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        StartCoroutine(CreatTail());
    }


    public IEnumerator CreatTail()
    {
        if (playerManager == null)
            yield return null;
        if(playerManager.Tails==null)
            yield return null;
        if (!playerManager.Loaded)
            yield return null;
        tailPos = playerManager.Tails;
        Tail_Normal = new GameObject[tailPos.Length];
        Tail_Speed = new GameObject[tailPos.Length];
        audioSource.volume = playerManager.volume * 0.3f;
        for (int i = 0; i < tailPos.Length; i++)//开始游戏时给加上拖尾，并隐藏掉
        {
            Tail_Normal[i] = Instantiate(TailEffects[0], tailPos[i].transform, false);
            Tail_Normal[i].transform.position = tailPos[i].transform.position;
            Tail_Normal[i].SetActive(false);
            Tail_Speed[i] = Instantiate(TailEffects[1], tailPos[i].transform, false);
            Tail_Speed[i].transform.position = tailPos[i].transform.position;
            Tail_Speed[i].SetActive(false);

        }
        print("tailPos.Length为"+ tailPos.Length + "产生了拖尾！");
    }
    void Start()
    {
        if (playerManager == null)
        {
            print("playerManager is null");
        }
        SetSpeed();
        IsRunOnWin = GameManger.Instance.IsRunOnWin;
        print(GameManger.Instance.IsRunOnWin);
        
    }
    void Update()
    {

        float x = Input.GetAxis(InputString_Tags.Horizontal);
        float y = Input.GetAxis(InputString_Tags.Vertical);
        dir = playerManager.dir;
        if (IsRunOnWin)
        {
            WinPlayerMove(x, y, dir);
        }
        else
        {
            AndroidPlayerMove(dir);
        }
        if (ETCInput.GetButton("SpeedUp"))
        {
            print("SpeedUP");
        }
    }
    #region 安卓平台移动
    void AndroidPlayerMove(Vector3 dir)
    {
        float x= ETCInput.GetAxis("Horizontal_Mobile");
        float y= ETCInput.GetAxis("Vertical_Mobile");
        Vector2 moveDir = new Vector2(x, y).normalized;
        Vector2 planeDir = new Vector2(dir.x, dir.y).normalized;
        if (Vector2.SignedAngle(planeDir, moveDir) > 5)
        {
             transform.Rotate(0, -Mathf.Abs(TurnAngle * Time.deltaTime)/4, 0);
        }
        else if (Vector2.SignedAngle(planeDir, moveDir) < -5)
        {
             transform.Rotate(0, Mathf.Abs( TurnAngle * Time.deltaTime)/4, 0);
        }

        if (x==0&&y==0)
        {
            for (int z = 0; z < tailPos.Length; z++)
            {
                Tail_Normal[z].SetActive(false);
            }

        }
        else
        {
            if (Vector2.SignedAngle(planeDir, moveDir) < 90 && Vector2.SignedAngle(planeDir, moveDir) > -90)
            {
                PlayerRig.AddForce(dir * Speed * 2.5f/2);
                for (int z = 0; z < tailPos.Length; z++)
                {
                    Tail_Normal[z].SetActive(true);
                }
            }
        }
        if (ETCInput.GetButton("SpeedUp") && playerManager.NowEnergy > 1)//加速
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
            PlayerRig.AddForce(dir  * Speed * 1.9f);
            playerManager.NowEnergy -= Time.deltaTime * (playerManager.SpeedUpConsume);
            for (int z = 0; z < tailPos.Length; z++)
            {
                Tail_Normal[z].SetActive(false);
                Tail_Speed[z].SetActive(true);
            }
        }
        else
        {
            audioSource.Stop();
            for (int z = 0; z < tailPos.Length; z++)
            {
                Tail_Speed[z].SetActive(false);
            }
        }
    }
    #endregion
    #region PC平台移动
    void WinPlayerMove(float x,float y,Vector3 dir)
    {
        if (y > 0)
        {
            if (Input.GetKey(KeyCode.LeftShift) && playerManager.NowEnergy > 1)
            {
                PlayerRig.AddForce(dir * y * Speed * 1.9f * 2.5f);
                playerManager.NowEnergy -= Time.deltaTime * (playerManager.SpeedUpConsume);
                for (int z = 0; z < tailPos.Length; z++)
                {
                    Tail_Normal[z].SetActive(false);
                    Tail_Speed[z].SetActive(true);
                }
            }
            else
            {
                PlayerRig.AddForce(dir * y * Speed * 2.5f);
                //PlayerRig.velocity = dir * y * Speed;
                for (int z = 0; z < tailPos.Length; z++)
                {
                    Tail_Normal[z].SetActive(true);
                    Tail_Speed[z].SetActive(false);
                }
            }
        }
        else
        {
            PlayerRig.AddForce((dir * y * Speed / 4) * 2.5f);
            //PlayerRig.velocity = dir * y * Speed/4;
            for (int z = 0; z < tailPos.Length; z++)
            {
                Tail_Normal[z].SetActive(true);
                Tail_Speed[z].SetActive(false);
            }
        }

        if (y == 0)
        {
            for (int z = 0; z < tailPos.Length; z++)
            {
                Tail_Normal[z].SetActive(false);
                Tail_Speed[z].SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) && y > 0)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
        //  print(IsSpeedUp);
        if (PlayerRig.velocity != Vector3.zero)
            transform.Rotate(0, x * TurnAngle * Time.deltaTime, 0);
    }
    #endregion
    public void ShowTail()
    {
        if(tailPos!=null)
        for (int z = 0; z < tailPos.Length; z++)
        {
            Tail_Normal[z].SetActive(true);
        }
    }
}
