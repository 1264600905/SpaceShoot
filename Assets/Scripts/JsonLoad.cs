using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;
public class JsonLoad : MonoBehaviour
{
    public List<plane> planes;
    public static JsonLoad instance;
    public bool LoadDonePlane = false;
    public bool LoadDoneOption = false;
    public List<LevelUpOption> levelUpOptionsList;
    private void Awake()
    {
        instance = this;
        //planeDefine();
       // SaveByJson();
        // LevelUpOptionDefine();
        // LevelUpOptionSaveByJson();
#if UNITY_STANDLONE_WIN
        LoadPlaneProperty();
        LoadLevelUpOptionProperty();
#elif UNITY_ANDROID
        StartCoroutine(LoadLevelUpOptionPropertyAndroid_WWW());
        StartCoroutine(LoadPlanePropertyAndroid_WWW());
#elif UNITY_EDITOR
        LoadPlaneProperty();
        LoadLevelUpOptionProperty();
#endif
        // CheckJson();
    }
    private void planeDefine()
    {
        planes = new List<plane>()
        {
            //Pistol
            new plane()
            {
                 PlaneName="rocket",
                 HP=100,
                 Level=1,
                 Exp2NextLevel=200,
                 Damage=5,
                 Speed=20,
                 TurnAngle=360,
                 AttackCD=0.35f,
                 Energy=100,
                 EnergyRecover=5,
                 ShootConsume=6,
                 SpeedUpConsume=7,
                 ammoType=0,
                 bulletSpeed=0.9f
                 
             }
        };
    }
    private void SaveByJson()
    {
        string filePath = Application.streamingAssetsPath + "/plane";
        string saveJsonStr = JsonMapper.ToJson(planes);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
    }
    private void LoadPlaneProperty()
    {
        planes = new List<plane>();
        string filePath = Application.streamingAssetsPath + "/plane";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            planes = JsonMapper.ToObject<List<plane>>(jsonStr);
        }
        if (planes.Count == 0)
        {
            Debug.Log("读取飞机Json文件失败");
        }
        else
        {
            //  Debug.Log("飞机Json文件读取成功!");
            LoadDonePlane = true;
        }
    }
    private IEnumerator LoadPlanePropertyAndroid_WWW()
    {
        planes = new List<plane>();
        string filePath = Application.streamingAssetsPath + "/plane";
        WWW www = new WWW(filePath);
        while (!www.isDone)
        {
            yield return null;
        }
        string jsonStr = www.text;
        planes = JsonMapper.ToObject<List<plane>>(jsonStr);
        if (planes.Count == 0)
        {
            Debug.Log("读取飞机Json文件失败");
        }
        else
        {
            //  Debug.Log("飞机Json文件读取成功!");
            LoadDonePlane = true;
        }
    }



    private void LevelUpOptionDefine()
    {
        levelUpOptionsList = new List<LevelUpOption>()
        {
            //血量升级
            new LevelUpOption()
            {
                 Name="结构优化1",
                 type=1,
                 ID=00011,
                 HP_Enhance=80
             }
        };
    }
    private void LevelUpOptionSaveByJson()
    {
        string filePath = Application.streamingAssetsPath + "/LevelUpOption";
        string saveJsonStr = JsonMapper.ToJson(levelUpOptionsList);
        StreamWriter sw = new StreamWriter(filePath);
        sw.Write(saveJsonStr);
        sw.Close();
    }
    private void LoadLevelUpOptionProperty()
    {
        levelUpOptionsList = new List<LevelUpOption>();
        string filePath = Application.streamingAssetsPath + "/LevelUpOption";
        if (File.Exists(filePath))
        {
            StreamReader sr = new StreamReader(filePath);
            string jsonStr = sr.ReadToEnd();
            sr.Close();
            levelUpOptionsList = JsonMapper.ToObject<List<LevelUpOption>>(jsonStr);
        }
        if (planes.Count == 0)
        {
            Debug.Log("读取飞机升级失败");
        }
        else
        {
            Debug.Log("飞机升级方向Json文件读取成功!"+ levelUpOptionsList[0].Name + levelUpOptionsList[0].HP_Enhance);
            LoadDoneOption = true;
        }
    }

    private IEnumerator LoadLevelUpOptionPropertyAndroid_WWW()
    {
        levelUpOptionsList = new List<LevelUpOption>();
        string filePath = Application.streamingAssetsPath + "/LevelUpOption";
        WWW www = new WWW(filePath);
        while (!www.isDone)
        {
            yield return null;
        }
        string jsonStr = www.text;
        levelUpOptionsList = JsonMapper.ToObject<List<LevelUpOption>>(jsonStr);
        if (levelUpOptionsList.Count == 0)
        {
            Debug.Log("读取飞机升级失败");
        }
        else
        {
            Debug.Log("飞机升级方向Json文件读取成功!" + levelUpOptionsList[0].Name + levelUpOptionsList[0].HP_Enhance);
            LoadDoneOption = true;
        }
    }
    public void CheckJson()
    {
        foreach(var p in levelUpOptionsList)
        {
            print(p.Name + "\n" + p.ID + "\n" + p.type);

        }


    }


    public LevelUpOption FindLevelUpOptionWithID(int id)
    {
        foreach(var p in levelUpOptionsList)
        {
            if (p.ID == id)
            {
                return p;
            }
        }
        return new LevelUpOption();
    }

    public plane FindPlaneByName(string name)
    {
        foreach(var p in planes)
        {
            if (p.PlaneName == name)
                return p;
        }
        return new plane();
    }

}
