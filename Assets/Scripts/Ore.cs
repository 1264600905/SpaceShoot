using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ore : MonoBehaviour
{
    public float ResourceNum;//资源量
    public float HealHp;//恢复血量
    public float HealEngery;//恢复能量

    private bool isMoveTarget = false;
    private Vector3 Target;
    private float speed;
    private void Update()
    {
        if (isMoveTarget)
        {
            GetComponent<Collider>().enabled = false;
            transform.Translate((Target * speed - transform.position) * Time.deltaTime,Space.World);
            if(Vector3.Distance(transform.position, Target) < 2f)
            {
                isMoveTarget = false;
                GetComponent<Collider>().enabled = true;
            }
        }
       


    }
    public void MoveToTargetPos1(Vector3 targetPos, float speed)
    {
        Target = targetPos;
        isMoveTarget = true;
        this.speed = speed;
    }

 

}
