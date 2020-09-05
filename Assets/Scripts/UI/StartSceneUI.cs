using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneUI : MonoBehaviour
{
    public Camera ImageCamera;
    public GameObject panel;
    private bool panelActive=false;
    public Text planeName;
    private planeStr[] planeStrs;
    private int nowShowPlane=0;
    private int choosePlane = 0;
    public Text Point;
    public GameObject Panel1;
    public bool Panel1State = false;

    public GameObject AboutPanel;
    public bool IsCloseAboutPanel=true;
    public Dropdown[] dropdowns;//0.按钮大小，1.敌人难度，2.游戏画质
    public Scrollbar volume;
    public Toggle FPS;
    void Start()
    {
        panel.SetActive(panelActive);
        planeStr str1 = new planeStr();
        str1.Name = "rocket";
        str1.pos = new Vector3(-140, 10, -235);
        planeStr str2 = new planeStr();
        str2.Name = "BlackPlane";
        str2.pos = new Vector3(-130, 10, -235);
        planeStr str3 = new planeStr();
        str3.Name = "GoldPlane";
        str3.pos = new Vector3(-120, 10, -235);
        planeStr str4 = new planeStr();
        str4.Name = "BlueFighter";
        str4.pos = new Vector3(-110, 10, -235);
        planeStrs = new planeStr[4]{str1,str2,str3,str4};
        ImageCamera.transform.position = planeStrs[nowShowPlane].pos;
        planeName.text = planeStrs[nowShowPlane].Name+" Now";
        PlayerPrefs.SetString("Plane", planeStrs[choosePlane].Name);
        SettingGet();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SettingSet()
    {
        PlayerPrefs.SetInt("ButtonSize", dropdowns[0].value);
        PlayerPrefs.SetInt("Enemy", dropdowns[1].value);
        PlayerPrefs.SetInt("GameQuality", dropdowns[2].value);
        PlayerPrefs.SetFloat("Volume",volume.value);
        PlayerPrefs.SetInt("FPS", FPS.isOn?1:0);
        GameManger.Instance.ShowFPS = (PlayerPrefs.GetInt("FPS") == 1) ? true : false;
        switch (dropdowns[2].value)
        {
            case 0:
                QualitySettings.SetQualityLevel(0);
                break;
            case 1:
                QualitySettings.SetQualityLevel(2);
                break;
            case 2:
                QualitySettings.SetQualityLevel(4);
                break;
            case 4:
                QualitySettings.SetQualityLevel(5);
                break;
        }
         
    }
    public void SettingGet()
    {
        dropdowns[0].value= PlayerPrefs.GetInt("ButtonSize");
        dropdowns[1].value=PlayerPrefs.GetInt("Enemy");
        dropdowns[2].value=PlayerPrefs.GetInt("GameQuality");
        volume.value=PlayerPrefs.GetFloat("Volume");
        FPS.isOn=(PlayerPrefs.GetInt("FPS")==1)?true:false;
        Point.text = "最佳成绩:" + PlayerPrefs.GetInt("Point", 0) + "级别" + PlayerPrefs.GetInt("Enemy");
    }

    public void OnStartButtonClick()
    {
        SceneManager.LoadScene("LoadScene");
    }

    public void OnSettingButtonClick()
    {
        if (!Panel1State) { 
            Panel1.SetActive(true);
            Panel1State = true;
            panel.SetActive(false);
            panelActive = false;
            SettingGet();
        }
        else
        {
            Panel1.SetActive(false);
            Panel1State = false;
            SettingSet();
        }
    }
    public void OnAirportButtonClick()
    {
        if (!panelActive)
        {
            Panel1.SetActive(false);
            Panel1State = false;
            panel.SetActive(true);
            panelActive = true;
        }
        else
        {
            panel.SetActive(false);
            panelActive = false;
        }
    }
    public void OnExitButtonClick()
    {
        Application.Quit();
    }

    public void OnLeftButton_click()
    {
        if (nowShowPlane == 0)
        {
            nowShowPlane = 3;
        }
        else
        {
            nowShowPlane--;
        }
        print(nowShowPlane);
        ImageCamera.transform.position = planeStrs[nowShowPlane].pos;
        if (nowShowPlane == choosePlane)
        {
            planeName.text = planeStrs[nowShowPlane].Name + " Now";
        }
        else
        {
            planeName.text = planeStrs[nowShowPlane].Name;
        }
    }

    public void OnRightButton_click()
    {
        if (nowShowPlane == 3)
        {
            nowShowPlane = 0;
        }
        else
        {
            nowShowPlane++;
        }
        print(nowShowPlane);
        ImageCamera.transform.position = planeStrs[nowShowPlane].pos;
        if (nowShowPlane == choosePlane)
        {
            planeName.text = planeStrs[nowShowPlane].Name + " Now";
        }
        else
        {
            planeName.text = planeStrs[nowShowPlane].Name;
        }
    }
    public void ChooseButtonClick()
    {
        choosePlane = nowShowPlane;
        PlayerPrefs.SetString("Plane", planeStrs[choosePlane].Name);
        planeName.text = planeStrs[choosePlane].Name + " Now";
        print("当前选择了项目" + planeStrs[choosePlane].Name);
    }


   private struct planeStr
    {
        public Vector3 pos;
        public string Name;
    }
   public void ClosePanel()
    {
        if (!panelActive)
        {
            panel.SetActive(true);
            panelActive = true;
        }
        else
        {
            panel.SetActive(false);
            panelActive = false;
        }
    }

    public void ClosePanel1()
    {
        Panel1.SetActive(false);
        Panel1State = false;
    }


    public void ChangeAboutPanel()
    {
        if (IsCloseAboutPanel)
        {
            AboutPanel.SetActive(true);
            IsCloseAboutPanel = false;
        }
        else
        {
            AboutPanel.SetActive(false);
            IsCloseAboutPanel = true;
        }
    }
}

