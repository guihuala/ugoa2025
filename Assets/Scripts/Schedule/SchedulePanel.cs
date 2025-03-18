using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SchedulePanel : SlidePanel
{
    [Header("组件配置")]
    public Button closeButton;

    public Button mission1Btn;
    public Button mission2Btn;
    public Button mission3Btn;
    
    public Button CGBtn;

    private void Start()
    {
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        
        CGBtn.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneName.CG,"..."));
        
        if(!SaveManager.Instance.isComplete)
            CGBtn.gameObject.SetActive(false);
    }
}
