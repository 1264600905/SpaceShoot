using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupputFuction : MonoBehaviour
{
    
    public static GameObject[] FindGameObjectsWithTagInChild(Transform transform, string tag)
    {
        List<Transform> list = new List<Transform>();
        List<GameObject> obj = new List<GameObject>();
        //子物体的子物体不是我的子物体！
        if (transform.childCount == 0)
        {
            Debug.Log("该物体下没有子物体");
            return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            list.Add(transform.GetChild(i));
        }
        foreach (var p in list)
        {

            if (p.tag==tag)
            {
                obj.Add(p.gameObject);
            }
        }
        if (obj.Count == 0)
        {
            Debug.Log("未找到相符子物体！");
            
        }
        GameObject[] gameObject = new GameObject[obj.Count];
        for(int z=0;z< obj.Count ; z++)
        {
            gameObject.SetValue(obj[z], z);
        }
       // print("FindGameObjectsWithTagInChild函数正常！长度为:"+gameObject.Length);
        return gameObject;
    }


     public static bool CreatOre(int resource,Vector3 pos ,int range)//指定位置生成宝石
    {
        pos = new Vector3(pos.x, pos.y, -5);
        for(int i = 0; i < resource; i++)
        {
            GameObject ore = GameManger.Instance.OrePrefabs[Random.Range(0, GameManger.Instance.OrePrefabs.Length)];
            GameObject OreObj = Instantiate(ore,pos, Quaternion.identity);
            OreObj.GetComponent<Ore>().MoveToTargetPos1(new Vector3(pos.x + Random.Range(-range, range), pos.y + Random.Range(-range, range), pos.z), 1f);
        }

        return true;
    }

    public static bool CreatOre(int resource, Vector3 pos, int temp, float OreGreen, float OreYellow)//指定位置生成宝石,按照颜色分类刷新概率在0到1;
    {
        pos = new Vector3(pos.x, pos.y, -2);
        for (int i = 0; i < resource; i++)
        {
            float f = Random.Range(0, 1f);
            GameObject ore;
            if (f <= OreGreen)
            {
                 ore= GameManger.Instance.OrePrefabs[Random.Range(0,2)];
            }
            else if(f<OreGreen+OreYellow)
            {
                ore = GameManger.Instance.OrePrefabs[Random.Range(2, 4)];
            }
            else
            {
                ore = GameManger.Instance.OrePrefabs[4];
            }
            GameObject OreObj = Instantiate(ore, pos, Quaternion.identity);
            if(GameManger.Instance!=null)
            OreObj.transform.SetParent(GameManger.Instance.OrePool.transform);
            OreObj.GetComponent<Ore>().MoveToTargetPos1(new Vector3(pos.x + Random.Range(-temp, temp), pos.y + Random.Range(-temp, temp), pos.z), 1f);
        }

        return true;
    }

    public static GameObject FindGameObjectWithTagInChild(Transform transform, string tag)//查找下面单个子物体
     {
        List<Transform> list = new List<Transform>();
        List<GameObject> obj = new List<GameObject>();
        //子物体的子物体不是我的子物体！
        if (transform.childCount == 0)
        {
            Debug.Log("该物体下没有子物体");
            return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            list.Add(transform.GetChild(i));
        }
        foreach (var p in list)
        {

            if (p.tag == tag)
            {
                obj.Add(p.gameObject);
            }
        }
        if (obj.Count == 0)
        {
            Debug.Log("未找到相符子物体！");

        }
        GameObject[] gameObject = new GameObject[obj.Count];
        for (int z = 0; z < obj.Count; z++)
        {
            gameObject.SetValue(obj[z], z);
        }
        print("FindGameObjectsWithTagInChild函数正常！长度为:" + gameObject.Length);
        return gameObject[0];
    }

    public static void EftPlay(GameObject eft,Transform pos)//规定地点播放特效
    {
        GameObject Eft = Instantiate(eft, PlayerShoot.pool.transform);
        Eft.transform.position = pos.position;
    }

    public static void EftPlayInParent(GameObject eft, Transform pos,bool followFather=false )//规定地点播放特效
    {
        GameObject Eft;
        if (!followFather)
            Eft = Instantiate(eft, PlayerShoot.pool.transform);
        else
        { Eft = Instantiate(eft, pos);
            Eft.transform.SetParent(pos);
        }
        Eft.transform.position = pos.position;
        
        eft.GetComponent<EFT_DestorySelf>().FollowParent = pos.gameObject;
    }
    public static void PlayAudio(AudioClip audioClip,Vector3 pos)
    {
        AudioSource.PlayClipAtPoint(audioClip, pos,0.8f);
    }


}
