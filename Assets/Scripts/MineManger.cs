using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MineManger : MonoBehaviour
{
    public string name;
    public double MaxHP;
    public int ResourseCount;
    public float GreenOre;
    public float YellowOre;


    private GameObject Effect;
    private float hpNow;
    private float currentShakeValue;
    private bool isShake = false;
    private float shakeValue=0.1f;
    private Vector3 Pos;
    private MeshRenderer renderer;
    private AudioClip audioClip;
    public TargetPos MinePos;

    private KeepRotation keepRotation;
    private bool OreRelease=false;
    public void InstallMine(string na, double hp, int Re)
    {
        name = na;
        MaxHP = hp;
        hpNow =(float) MaxHP;
        ResourseCount = Re;
    }

    private void Start()
    {
        //InstallMine("Rock", 100, 15);
        keepRotation = GameObject.FindGameObjectWithTag("MiniMap").GetComponent<KeepRotation>();
        hpNow = (float)MaxHP;
        audioClip = GameManger.Instance.MineDestoryAudio;
        Effect = GameManger.Instance.MineDestoryEft;
        renderer = GetComponent<MeshRenderer>();
        Pos = transform.position;
    }
    private void Update()
    {

        Shake();
    }
    public void TakeDamage(float damage,float Damage2Mine)
    {
       
        hpNow -=damage *(1+ Damage2Mine);
        isShake = true;
        if (hpNow <= 0)
        {
            SupputFuction.EftPlay(Effect, transform);
            SupputFuction.PlayAudio(audioClip, transform.position);
            if(keepRotation!=null)
                keepRotation.enabled = false;
            Destroy(gameObject, 0f);
            if (!OreRelease)
            {
                SupputFuction.CreatOre(ResourseCount, transform.position, 9, GreenOre, YellowOre);
                OreRelease = true;
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == InputString_Tags.PlayerTag)
        {
            collision.gameObject.GetComponent<PlayerManager>().TakeDamage(5);
            print("碰撞伤害为：" + 5f);

        }
    }


    void Shake()
    {
        if (isShake)
        {
            
            //先左右震动再上下震动
            transform.position = new Vector3((transform.localPosition.x+Random.Range(-currentShakeValue, currentShakeValue)), transform.localPosition.y, transform.localPosition.z);
            transform.position = new Vector3(transform.localPosition.x, transform.localPosition.y+(Random.Range(-currentShakeValue, currentShakeValue)), transform.localPosition.z);
            renderer.material.color = Color.red;
            currentShakeValue /= 1.2f;//震动数值减少
            if (currentShakeValue <= 0.05f)
            {
                Pos = transform.position;
                isShake = false;
                currentShakeValue = shakeValue;
                renderer.material.color = Color.white;
            }
        }
        else
        {
            currentShakeValue = shakeValue;
        }



    }



    private void OnDestroy()
    {
        MapManger.instance.DeleteMine(MinePos);



    }

}
