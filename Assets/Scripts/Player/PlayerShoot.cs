using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    private GameObject[] ShootPos;
    private GameObject[] bullets;//颜色表示伤害等级0. 蓝色1.黄色2.红色
    private float shootCD=0;
    public static GameObject pool;
    public AudioClip shoot;
    private PlayerManager playerManager;
    private void Awake()
    {
      pool=  GameObject.Find("BulletPool");
        StartCoroutine(GetPlayerManager());
    }
    private IEnumerator GetPlayerManager()
    {
        playerManager = GetComponent<PlayerManager>();

        if (!playerManager.Loaded)
        {
            yield return null;
        }
        ShootPos = playerManager.shootPos;
        bullets = playerManager.bulletPrefabs;
    }
    private void Start()
    {

        // Debug.Log(ShootPos.Length);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space)||ETCInput.GetButton("Shoot"))
        {
            if (shootCD >= playerManager.AttackCD*(1-playerManager.AttackSpeedEnhance)&&playerManager.NowEnergy>playerManager.ShootConsume)
            {
                foreach (var pos in ShootPos)
                {
                   
                    GameObject bullet = Instantiate(bullets[0], pool.transform,false);
                    bullet.transform.localRotation = Quaternion.Euler(PlayerMove.instance.transform.eulerAngles);
                    bullet.transform.position = pos.transform.position;
                    bullet.GetComponent<Bullet>().SetPlayer(this.gameObject,(float)( playerManager.Damage + playerManager.DamageEnhance));
                    AudioSource.PlayClipAtPoint(shoot, pos.transform.position,PlayerManager.instance.volume);
                }
                //Debug.Log(ShootPos==null?"ShootPos为空":"正常");
                shootCD = 0;
                playerManager.NowEnergy -= playerManager.ShootConsume;
            }
        }
        shootCD += Time.deltaTime;
    }

    //public void ShowShoot()
    //{
    //    if (shootCD >= 0.6)
    //    {
    //        foreach (var pos in ShootPos)
    //        {

    //            GameObject bullet = Instantiate(bullets[0], pool.transform, false);
    //            bullet.transform.localRotation = Quaternion.Euler(PlayerMove.instance.transform.eulerAngles);
    //            bullet.transform.position = pos.transform.position;
    //            bullet.GetComponent<Bullet>().SetPlayer(this.gameObject, 0);
    //            AudioSource.PlayClipAtPoint(shoot, pos.transform.position, PlayerManager.instance.volume);
    //        }
    //        shootCD = 0;
    //    }
    //    shootCD += Time.deltaTime;
    //}
}
