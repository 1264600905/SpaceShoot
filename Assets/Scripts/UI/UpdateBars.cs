using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class UpdateBars : MonoBehaviour
{
    public Slider EngerySlider;
    public Slider LifeSlider;
    public Text EngeryText;
    public Text LifeText;
    public Text TextLevel;
    public Text Resoure;
    public Slider EXPBar;

    public  GameObject EnemyPrefab;
    private EnemyManger EM;
    private PlayerManager PM;
    private float UpdateTimer;
    public bool IsEnemy = false;
    private Quaternion StartRotation;
    void Start()
    {
        if (!IsEnemy){
            PM = PlayerManager.instance;
        }
        else 
        {
            EM= EnemyPrefab.GetComponent<EnemyManger>();
            EXPBar.enabled = false;   
            IsEnemy = true;
            StartRotation = transform.rotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsEnemy)
        {
            PM = PlayerManager.instance;
        }
        else
        {
            transform.rotation = StartRotation;
        }

        if (UpdateTimer > 0.5)
        {
            UpdateTimer = 0;
            UpdateUI();
        }
        else
        {
            UpdateTimer += Time.deltaTime;
        }
    }


    public void UpdateUI()
    {
        if (IsEnemy) {
            if (EM == null)
                return;
            EngerySlider.value = (float)(EM.NowEnergy / EM.MaxEnergy);
            LifeSlider.value = (float)(EM.NowHP / EM.MaxHP);
            EngeryText.text = (int)EM.NowEnergy + " /" + (int)EM.MaxEnergy;
            LifeText.text = (int)EM.NowHP + "/" + (int)EM.MaxHP;
            Resoure.text = (int)EM.MineExp + "";
            TextLevel.text = EM.Level + "";
        }
        else
        {
            if (PM == null)
                return;
            EngerySlider.value = (float)(PM.NowEnergy / PM.MaxEnergy);
            LifeSlider.value = (float)(PM.NowHP / PM.MaxHP);
            EngeryText.text = (int)PM.NowEnergy + " /" + (int)PM.MaxEnergy;
            LifeText.text = (int)PM.NowHP + "/" + (int)PM.MaxHP;
            Resoure.text = (int)PM.MineExp + "";
            EXPBar.value = (float)(PM.MineExp / PM.Exp2NextLevel);
            TextLevel.text = PM.Level + "";
        }

    }
}
