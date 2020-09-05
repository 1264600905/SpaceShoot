using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDesstory : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDesstory.DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
