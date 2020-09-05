using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour
{
	public float speed;
    public float destoryTime = 1f;
    public GameObject HitEffect;

    private float tempTime = 0f;
    private float damage;
    private GameObject Player;
    private float timer;
    private float RigSetActiveDistance = 4f;
    private Collider collider;
    private Vector3 StartPos;
    public void SetPlayer(GameObject gameObject,float da)
    {
        Player = gameObject;
        damage = da;
    }
	void Start ()
	{
		GetComponent<Rigidbody>().velocity =PlayerManager.instance.dir*speed*(float)(PlayerManager.instance.bulletSpeed*(1+ PlayerManager.instance.BulletSpeedenhance));
        collider = GetComponent<Collider>();
        collider.enabled = false;
        StartPos = transform.position;
    }
    private void Update()
    {
        if (tempTime > destoryTime)
        {
            Destroy(gameObject);
        }
        else
           tempTime += Time.deltaTime;
        if(PlayerManager.instance!=null)
        if (Vector3.Magnitude(Player.transform.position-transform.position)>RigSetActiveDistance)
        {
            collider.enabled = true;
        }

        if (Vector3.Distance(StartPos, transform.position) > (PlayerManager.instance.ShootDistance)*(1+PlayerManager.instance.ShootDistanceEnhance))
        {
            Destroy(gameObject,0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);

        switch (collision.transform.tag)
        {
            case InputString_Tags.MineTag:
                collision.gameObject.GetComponent<MineManger>().TakeDamage((float)(PlayerManager.instance.Damage+PlayerManager.instance.DamageEnhance), (float)PlayerManager.instance.Damage2Mine);
                GameObject eft = Instantiate(HitEffect);
                eft.transform.SetParent(PlayerShoot.pool.transform);
                eft.transform.position = transform.position;
                Destroy(this.gameObject, 0f);
                break;

            case InputString_Tags.EnemyTag:
                GameObject eft2 = Instantiate(HitEffect);
                eft2.transform.SetParent(PlayerShoot.pool.transform);
                EnemyManger EM=  collision.gameObject.GetComponent<EnemyManger>();
                EM.TakeDamage((float)(PlayerManager.instance.Damage + PlayerManager.instance.DamageEnhance));

                eft2.transform.position = transform.position;
                Destroy(this.gameObject, 0f);
                break;
            case InputString_Tags.PlayerTag:
                break;
            default:

                GameObject eft1 = Instantiate(HitEffect);
                eft1.transform.SetParent(PlayerShoot.pool.transform);
                eft1.transform.position = transform.position;
                Destroy(this.gameObject, 0f);
                break;

        }
    }


}
