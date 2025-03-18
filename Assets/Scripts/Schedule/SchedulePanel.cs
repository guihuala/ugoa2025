using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchedulePanel : SlidePanel
{
    [Header("按钮配置")]
    public Button closeButton;
    public Button CGBtn;
    
    [Header("任务配置")]
    public Button mission1Btn;
    public Button mission2Btn;
    public Button mission3Btn;

    [Header("面板配置")] 
    public Transform mission1Info;
    public Transform mission2Info;
    public Transform mission3Info;
    
    private void Start()
    {
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        CGBtn.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneName.CG,"..."));

        mission1Btn.onClick.AddListener(() => { OpenInfo(mission1Info); });
        mission2Btn.onClick.AddListener(() => { OpenInfo(mission2Info); });
        mission3Btn.onClick.AddListener(() => { OpenInfo(mission3Info); });
        
        InitUI();
    }

    void InitUI()
    {
        if(!SaveManager.Instance.isComplete)
            CGBtn.gameObject.SetActive(false);
        
        mission1Info.gameObject.SetActive(false);
        mission2Info.gameObject.SetActive(false);
        mission3Info.gameObject.SetActive(false);
    }

    void OpenInfo(Transform info)
    {
        info.gameObject.SetActive(true);
    }
}
