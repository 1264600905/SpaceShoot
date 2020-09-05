using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class LoadScene : MonoBehaviour
{
    private bool LoadDone = false;
    private AsyncOperation AO;
    private void Awake()
    {
        
    }
    private void Start()
    {
        AO = SceneManager.LoadSceneAsync("MainScene");
        AO.allowSceneActivation = false;
    }
    private void Update()
    {
        if (AO.progress>=0.9f)
        {
            GetComponent<Text>().text = "加载完成！按任意键继续！";
            if (Input.anyKeyDown)
            {
                AO.allowSceneActivation = true;
            }
        }
    }


}
