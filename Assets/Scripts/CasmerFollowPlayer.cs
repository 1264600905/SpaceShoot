using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CasmerFollowPlayer : MonoBehaviour
{
    public Transform playerPos;
    public int cameraHeight = 26;
    public Vector3 Temp;
    // Start is called before the first frame update
    private void Awake()
    {
    }
    void Start()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if(playerPos!=null)
        this.transform.position = new Vector3(Temp.x + playerPos.position.x, Temp.y + playerPos.position.y, -cameraHeight+playerPos.position.z);
    }
}
