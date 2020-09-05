using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HedgehogTeam.EasyTouch;
using UnityEngine.UI;
public class GameManger : MonoBehaviour
{
    public  GameObject[] OrePrefabs;
    public static GameManger Instance;
    public AudioClip MineDestoryAudio;
    public GameObject MineDestoryEft;
    public GameObject OrePool;
    public GameObject BulletPool;
    public List<LevelUpOption> levelUpOptionsList;
    public List<plane> planes;
    public List<GameObject> enemyList;
    public bool IsRunOnWin ;
    public bool ShowFPS = false;
    public GameObject FPS_UI;
    private Text FpsText;
    public GameObject[] Mobile_UI;
    public float FPSTimer;
    public GameObject DeathEft;
    public float ReBornTimer = 0;
    public bool PlayerIsDeath = false;
    public AudioClip PlayerDeathAudio;
    public int PlayerLevel;

    private bool IsRun = false;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(this);//游戏管理器第一个场景便存在一直到游戏结束
        Application.targetFrameRate = 60;
#if UNITY_STANDLONE_WIN
        IsRunOnWin=true;
         foreach(var p in Mobile_UI)
        {
          p.SetActive(false);
        }
#elif UNITY_ANDROID
        IsRunOnWin =false;
#elif UNITY_EDITOR
        IsRunOnWin = false;
#endif
    }
    private void Start()
    {
        StartCoroutine(LoadJsonCheck());
        if (ShowFPS)
        {
           
            FpsText = Instantiate(FPS_UI).GetComponent<Text>();
            FpsText.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
            FpsText.gameObject.transform.position = GameObject.Find("Canvas").transform.position;
        }
    }
   private  IEnumerator LoadJsonCheck()
    {

        if (!(JsonLoad.instance.LoadDonePlane && JsonLoad.instance.LoadDoneOption))
        {
            yield return null;
        }
        levelUpOptionsList = JsonLoad.instance.levelUpOptionsList;
        planes = JsonLoad.instance.planes;
        print("LevelUpOpti0n有：" + levelUpOptionsList.Count + "个" + "\n");
        print("plane有：" + planes.Count + "个" + "\n");
    }

    private void Update()
    {
        if (PlayerManager.instance != null)
        {
            PlayerLevel = PlayerManager.instance.Level;
        }
        if (ShowFPS&& FpsText == null)
        {
            FpsText = Instantiate(FPS_UI).GetComponent<Text>();
            FpsText.gameObject.transform.SetParent(GameObject.Find("Canvas").transform);
            Vector3 V3= GameObject.Find("Canvas").transform.position;
            FpsText.rectTransform.position = new Vector3(-80,-40,V3.z-1);
        }
        if (FPSTimer < 0.2&& ShowFPS)
        {
            FPSTimer += Time.deltaTime;
        }
        else
        {
            FPSTimer = 0;
            if (ShowFPS && FpsText != null)
            {
                float FPS_temp = 1 / Time.deltaTime;
                FpsText.text = "FPS:" + FPS_temp;
            }
        }

        if (PlayerIsDeath)
        {
            if (!IsRun&&ReBornTimer>2f)
            {
                OnPlayerDeath();
                IsRun = true;
            }
            if (ReBornTimer >4)
            {
               GameObject player= MapManger.instance.CreatePlayer(PlayerLevel);
                PlayerManager.instance= player.GetComponent<PlayerManager>();
                UI_Manger.instance.UpdateUI();
                PlayerIsDeath = false;
                IsRun = false;
                ReBornTimer = 0;
            }
            else
            {
                ReBornTimer += Time.deltaTime;
            }
        }
    }
    private void OnPlayerDeath()
    {
        if (UI_Manger.instance.Point > PlayerPrefs.GetInt("Point"))
        {
            PlayerPrefs.SetInt("Point", UI_Manger.instance.Point);
        }
        UI_Manger.instance.DeathShowPointText.text = "得分为：" + UI_Manger.instance.Point;
        UI_Manger.instance.Point = 0;
        UI_Manger.instance.OnPauseButtonClick();
        UI_Manger.instance.DeathShowPointText.enabled = true;
    }

}
