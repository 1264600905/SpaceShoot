using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepRotation : MonoBehaviour
{
    private Quaternion Rotation;
    private float timer;
    private void Awake()
    {
        Rotation = transform.rotation;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Rotation;
    }
}
