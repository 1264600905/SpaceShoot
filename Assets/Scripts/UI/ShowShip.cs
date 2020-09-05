using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowShip : MonoBehaviour
{
    public bool isShow = false;
    private PlayerManager GM;
    private void Start()
    {
        GM = GetComponent<PlayerManager>();
    }
    private void Update()
    {
        if (isShow)
        {
           // GetComponent<PlayerShoot>().ShowShoot();
            GetComponent<PlayerMove>().ShowTail();
        }
    }
}
