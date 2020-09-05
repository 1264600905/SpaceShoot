using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    public float speed=25;
    public float destoryTime = 1f;
    public GameObject HitEffect;

    private float tempTime = 0f;
    private float damage;
    private GameObject EnemyObj;
    private float timer;
    private float RigSetActiveDistance = 4f;
    private Collider collider;
    private Vector3 StartPos;
    private bool IsSet = false;
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
        StartPos = transform.position;
    }

    public void Shoot(GameObject Obj,Vector3 dir ,float damage)
    {
        EnemyObj = Obj;
        GetComponent<Rigidbody>().velocity = dir * speed;
        this.damage = damage;
        IsSet = true;
    }
    private void Update()
    {
        if (!IsSet)
        {
            return;
        }
        if (tempTime > destoryTime)
        {
            Destroy(gameObject);
        }
        else
            tempTime += Time.deltaTime;
        if(EnemyObj != null)
        if (Vector3.Magnitude(EnemyObj.transform.position - transform.position) > RigSetActiveDistance)
        {
            collider.enabled = true;
        }

        if (Vector3.Distance(StartPos, transform.position) > 15)
        {
            Destroy(gameObject, 0.2f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        print(collision.gameObject.name);

        switch (collision.transform.tag)
        {
            case InputString_Tags.MineTag:
                collision.gameObject.GetComponent<MineManger>().TakeDamage(damage,1f);
                GameObject eft = Instantiate(HitEffect);
                eft.transform.SetParent(PlayerShoot.pool.transform);
                eft.transform.position = transform.position;
                Destroy(this.gameObject, 0f);
                break;
            case InputString_Tags.PlayerTag:
                GameObject eft2 = Instantiate(HitEffect);
                collision.gameObject.GetComponent<PlayerManager>().TakeDamage(damage);
                eft2.transform.SetParent(PlayerShoot.pool.transform);
                eft2.transform.position = transform.position;
                Destroy(this.gameObject, 0f);
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
