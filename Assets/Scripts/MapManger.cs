using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManger : MonoBehaviour
{
    public static MapManger instance;
    public int MapHeight;
    public int MapWidth;
    public GameObject[] Walls;//空气墙上下左右
    public GameObject[] Mines;//所有的矿石预制体
    public float[] MineCreatProbability;//矿石生成概率 0.蓝色矿石 1.红色矿石 2.大红矿石  3.小红矿石 4.陨石1 5.陨石2 6.陨石3
    public List<TargetPos> MineExist;//当前存在的矿石Pos
    private float UpdateMapTimer;//地图更新计时器
    public float UpdateMapTime=5f;
    public int MinResourseMineCount = 30;

    public GameObject[] planes;


    public float density=1;
    private int MineResourseNum = 0;
    public float times;
    public GameObject BG;//背景大小为320*320 
    // Start is called before the first frame update

    public GameObject[] EnemyPrefabs;
    private void Awake()
    {
        instance = this;
        MineExist = new List<TargetPos>();
        CreatePlayer(1);
        CreatEnemy(1, 200,20);
        CreatWall();
        CreatMine(true);

    }
    void Start()
    {

    }

    public void CreatEnemy(int level,int Res,int num)
    {
        for (int i = 0; i < num; i++)
        {
            int x= Random.Range(0, EnemyPrefabs.Length);
            GameObject Obj = CreatGameObjectInMap(EnemyPrefabs[x], 1);
            EnemyManger EM = Obj.GetComponent<EnemyManger>();
            EM.CreateEnemy(level, Res);
            GameManger.Instance.enemyList.Add(EM.gameObject);
        }
    }

    private void CreatWall()
    {
        Walls[0].transform.position = new Vector3(0, MapHeight / 2, -4);
        Walls[0].transform.localScale = new Vector3(MapWidth, 1, 10);
        Walls[1].transform.position = new Vector3(0, -MapHeight / 2, -4);
        Walls[1].transform.localScale = new Vector3(MapWidth, 1, 10);
        Walls[2].transform.position = new Vector3(-MapWidth / 2,0, -4);
        Walls[2].transform.localScale = new Vector3(1, MapHeight, 10);
        Walls[3].transform.position = new Vector3(MapWidth / 2, 0, -4);
        Walls[3].transform.localScale = new Vector3(1, MapHeight, 10);
    }


    private void CreatMine(bool isFirstCreat,int num=20)//随机生成地图
    {
        int x =(int)Mathf.Floor( MapWidth / density);
        int y =(int)Mathf.Floor(MapHeight / density);
        print("当前地图密度为:" + density + "\nx的值为：" + x + "\ny的值为:" + y);
        TargetPos tempPos;
        if (isFirstCreat)
        {
            for (int a = 0; a <= x; a++)
            {
                for (int b = 0; b <= y; b++)
                {
                    int type = Random.Range(0, 7);
                    float temp = Random.Range(0, 1f);
                    if (temp < MineCreatProbability[type])
                    {
                        GameObject mine = Instantiate(Mines[type], transform, true);
                        mine.transform.position = new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4);
                        // print("矿石(" + a + "," + b + ")位置为:" + new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4));
                        tempPos.x = b; tempPos.y = a; tempPos.mineType = type;
                        mine.GetComponent<MineManger>().MinePos = tempPos;
                        MineExist.Add(tempPos);

                    }

                }
            }

        }else{
            for(int q = 0; q < num; q++)
            {
                int a = Random.Range(0, x);
                int b = Random.Range(0, y);
                bool isExist = false;
                foreach(var e in MineExist)
                {
                    if (e.x == a&&e.y == b){
                        isExist = true;
                     }
                }
                if (isExist == false)
                {
                    int type = Random.Range(0, 7);
                    float temp = Random.Range(0, 1f);
                    if (temp < MineCreatProbability[type])
                    {
                        GameObject mine = Instantiate(Mines[type], transform, true);
                        mine.transform.position = new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4);
                        // print("矿石(" + a + "," + b + ")位置为:" + new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4));
                        tempPos.x = b; tempPos.y = a; tempPos.mineType = type;
                        mine.GetComponent<MineManger>().MinePos = tempPos;
                        MineExist.Add(tempPos);
                    }
                }
            }
        }
    }


    public GameObject CreatGameObjectInMap(GameObject gameObject,int type)//一个在地图上生成物体的通用方法
    {
        int x = (int)Mathf.Floor(MapWidth / density);
        int y = (int)Mathf.Floor(MapHeight / density);
        int a = Random.Range(0, x);
        int b = Random.Range(0, y);
        TargetPos tempPos;
        bool isExist = false;
        foreach (var e in MineExist)
        {
            if (e.x == a && e.y == b)
            {
                isExist = true;
            }
        }
        if (isExist)
        {
           return CreatGameObjectInMap(gameObject,type);
        }
        else
        {
            GameObject Obj = Instantiate(gameObject, transform, true);
            Obj.transform.position = new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4);
            
            // print("矿石(" + a + "," + b + ")位置为:" + new Vector3(-MapWidth / 2 + density * b, -MapHeight / 2 + density * a, -4));
            tempPos.x = b; tempPos.y = a; tempPos.mineType = type;
            //mine.GetComponent<MineManger>().MinePos = tempPos;
            MineExist.Add(tempPos);
            return Obj;
        }

    }



    public void DeleteMine(TargetPos pos)
    {
        MineExist.Remove(pos);
      //  print("删除的矿物相对坐标为:"+pos.x + "," + pos.y);
    }
    

    public GameObject CreatePlayer(int level)
    {
       string planeStr= PlayerPrefs.GetString("Plane");
       // print("PlayerPrefs中“Plane”为:" + planeStr);
       for(int i = 0; i < planes.Length; i++)
        {
           // print("当前的P中PlaneName为:" + planes[i].GetComponent<PlayerManager>().PlaneName);
            if (planes[i].GetComponent<PlayerManager>().PlaneName == planeStr)
            {
                GameObject P=CreatGameObjectInMap(planes[i], 10);
                P.GetComponent<PlayerManager>().HpChange(10000f);
                P.GetComponent<PlayerManager>().EngeryChange(10000f);
                for(int a=0;a< level; a++)
                {
                    P.GetComponent<PlayerManager>().LevelUp(true);
                }
                return P;
               // UI_Manger.instance.UpdateUI();
            }
        }
        return null;
    }




    // Update is called once per frame
    void Update()
    {
        
        if (UpdateMapTimer > UpdateMapTime)
        {
            UpdateMapTimer = 0f;
            foreach(var p in MineExist)
            {
                if (p.mineType < 4)
                {
                    MineResourseNum++;
                }
            }
            if (GameManger.Instance.enemyList.Count < 20)
            {
                CreatEnemy(PlayerManager.instance.Level + 2,(int)100+(PlayerManager.instance.Level + 2)*50, 20 - GameManger.Instance.enemyList.Count);
            }
            print("当前资源矿石数量为：" + MineResourseNum);
            if (MineResourseNum < MinResourseMineCount)
            {
                
                CreatMine(false);
            }
        }
        else
        {
            UpdateMapTimer += Time.deltaTime;
        }
        MineResourseNum = 0;



    }



}

public struct TargetPos{
    public int x;
    public int y;
    public int mineType;
 }