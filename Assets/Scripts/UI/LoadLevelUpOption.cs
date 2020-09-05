using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LoadLevelUpOption : MonoBehaviour
{
    public Text btnText0;
    public Text btnText1;
    public Text btnText2;
    public Button button0;
    public Button button1;
    public Button button2;
    public Image CountBG;
    public Text Count;


    private int Btn0;
    private int Btn1;
    private int Btn2;
    private List<LevelUpOption> OptionsChosed;
    private List<LevelUpOption> levelUpOptions;
    private bool isShow = true;
    private int count=0;


    public static LoadLevelUpOption instance;
    private void Start()
    {
        instance = this;
        OptionsChosed = new List<LevelUpOption>();
        levelUpOptions = GameManger.Instance.levelUpOptionsList;
        Show(false);
    }


    public void ShowLevelUpOption(bool IsOverlay)//bool值表示叠加
    {
        if (IsOverlay)
        {
            count++;
        }
        int ID0 = SelectLevelUpOptions();
        int ID1= SelectLevelUpOptions();
        int ID2= SelectLevelUpOptions();
        LevelUpOption Option0;
        LevelUpOption Option1;
        LevelUpOption Option2;
        if ((ID0 == ID2) || (ID0 == ID2))
        {
            ID0 = SelectLevelUpOptions();
        }
        if ((ID2 == ID1) || (ID2 == ID0))
        {
            ID2 = SelectLevelUpOptions();
        }
        Option0 = JsonLoad.instance.FindLevelUpOptionWithID(ID0);
        Option1 = JsonLoad.instance.FindLevelUpOptionWithID(ID1);
        Option2 = JsonLoad.instance.FindLevelUpOptionWithID(ID2);
        btnText0.text = Option0.Name;
        btnText1.text = Option1.Name;
        btnText2.text = Option2.Name;
       // print(Option0.ID +""+ Option1.ID +""+ Option2.ID);
        Btn0 = ID0;
        Btn1 = ID1;
        Btn2 = ID2;
        
        Show(true);

    }

    public void Btn0Click()
    {
        PlayerManager.instance.ChooseLevelUpOption(Btn0);
        OptionsChosed.Add(JsonLoad.instance.FindLevelUpOptionWithID(Btn0));
        count--;
        Show(false);
        if (count > 0)
        {
            ShowLevelUpOption(false);
        }
       
    }
    public void Btn1Click()
    {
        PlayerManager.instance.ChooseLevelUpOption(Btn1);
        OptionsChosed.Add(JsonLoad.instance.FindLevelUpOptionWithID(Btn1));
        count--;
        Show(false);
        if (count > 0)
        {
            ShowLevelUpOption(false);
        }
       
    }

    public void Btn2Click()
    {
        PlayerManager.instance.ChooseLevelUpOption(Btn2);
        OptionsChosed.Add(JsonLoad.instance.FindLevelUpOptionWithID(Btn2));
        count--;
        Show(false);
        if (count > 0)
        {
            ShowLevelUpOption(false);
        }
    }
    private int SelectLevelUpOptions()
    {
        int num = Random.Range(0, 13)+1;
       // print("num数值为："+num);
        int id=0;
        LevelUpOption levelUpOptions;
        int maxId=0;
        foreach (var p in OptionsChosed)
        {
            if (p.type == num&&p.ID>maxId)
            {
                maxId = p.ID;
            }
        }
        if (maxId == 0)
        {
            id = num * 10 + 1;
        }
        else
        {
            id = maxId + 1;
        }
        levelUpOptions = JsonLoad.instance.FindLevelUpOptionWithID(id);
        if (levelUpOptions.ID == 0)
        {
            id = SelectLevelUpOptions();
        }
       // print("当前查询的id:"+id);
        return id;
    }
    void Show(bool x)
    {
            btnText0.enabled = x;
            btnText1.enabled = x;
            btnText2.enabled = x;
        button0.gameObject.SetActive(x);
        button1.gameObject.SetActive(x);
        button2.gameObject.SetActive(x);
        if (count >= 2)
        {
            CountBG.gameObject.SetActive(x);
            Count.text = count+"";
            Count.enabled = x;
        }
        else
        {
            CountBG.gameObject.SetActive(false);
            Count.enabled =false;
        }
       // print(count);
    }

}
