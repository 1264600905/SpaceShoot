using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class UI_Manger : MonoBehaviour
{
    public static UI_Manger instance;
    public GameObject UI_Bar;
    public int KillCount = 0;
    public int Point = 0;
    public Text PointText;
    public GameObject PausePanel;
    private bool IsShowPausePanel = false;
    public Text DeathShowPointText;



    public GameObject[] Yaogan;
    // Start is called before the first frame update
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        SetButtonSize();
    }
    void Start()
    {
        
    }

    public void UpdatePointText(int Kill,int Point)
    {
        KillCount += Kill;
        this.Point += Point;
        string s = "击败：" + KillCount + " 得分:" + this.Point;
        PointText.text = s;
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateUI()
    {
        UI_Bar.GetComponent<UpdateBars>().UpdateUI();
    }


    public void OnPauseButtonClick()
    {
        if (IsShowPausePanel)
        {
            Time.timeScale = 1f;
            PausePanel.SetActive(false);
            IsShowPausePanel = false;
            DeathShowPointText.enabled = false;
        }
        else
        {
            Time.timeScale = 0f;
            PausePanel.SetActive(true);
            IsShowPausePanel = true;
            
        }
    }

    public void OnHomeButtonClick()
    {
        Destroy(GameManger.Instance);
        SceneManager.LoadScene(0);
        Time.timeScale = 1f;
    }
    public void OnRetryButtonClick()
    {
        SceneManager.LoadScene(1);
        Time.timeScale = 1f;
    }


    private  void SetButtonSize()
    {
        float i = 1;
        switch (PlayerPrefs.GetInt("ButtonSize"))
        {
            case 0:

                break;
            case 1:
                i = 1.25f;
                break;
            case 2:
                i = 1.5f;
                break;

        }
        foreach (var p in Yaogan)
        {
            p.gameObject.transform.localScale = i*new Vector3(1,1,1);
         }
    }
}
