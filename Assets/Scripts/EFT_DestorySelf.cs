using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EFT_DestorySelf : MonoBehaviour
{
    public float DestoryTime = 2f;
    private float Timer=0;
    public GameObject FollowParent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (FollowParent != null)
        {
            transform.position = FollowParent.transform.position;
        }
        if (Timer > DestoryTime)
        {
            Destroy(gameObject);
        }
        else
        {
            Timer += Time.deltaTime;
        }
    }
}
