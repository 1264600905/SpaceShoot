using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateSelf : MonoBehaviour
{
    public bool startRotate;
    public float rotateSpeed;
    private float RotateX ;
    private float RotateY;
    private float RotateZ;
    // Update is called once per frame
    private void Start()
    {
        RotateX = Random.Range(1,20);
        RotateY = Random.Range(1, 20);
        RotateZ = Random.Range(1, 20);
    }
    void Update()
    {
        if (startRotate)
        {
           
            transform.Rotate(new Vector3(RotateX * Time.deltaTime*rotateSpeed, RotateY * Time.deltaTime*rotateSpeed, RotateZ * Time.deltaTime*rotateSpeed));
        }
        
    }
}
