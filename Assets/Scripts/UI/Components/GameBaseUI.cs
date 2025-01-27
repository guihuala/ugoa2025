using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBaseUI : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Text stepText;

    private void Awake()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPanel("SettingPanel");
        });
    }

    private void Start()
    {
        EVENTMGR.UseSteps += UpdateStepText;
    }

    private void UpdateStepText(int remainSteps)
    {
        stepText.text = "当前步数：" + remainSteps.ToString();
    }
}
