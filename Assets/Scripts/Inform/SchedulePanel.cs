using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SchedulePanel : SlidePanel
{
    [Header("按钮配置")]
    public Button closeButton;
    public Button CGBtn;
    
    [Header("任务配置")]
    public Button mission1Btn;
    public Button mission2Btn;
    public Button mission3Btn;

    public Transform mission1Transform;
    public Transform mission2Transform;
    public Transform mission3Transform;
    
    [Header("面板配置")] 
    public Transform missionInfo;
    public Text missionName;
    public Text missionDescription;
    
    [Header("游戏次数统计")]
    public Text playTimeText;
    public Text failureTimeText;
    
    [Header("动画配置")]
    public float infoFadeDuration = 0.2f;
    public float infoScaleDuration = 0.3f;
    public Ease infoScaleEase = Ease.OutBack;
    public Vector3 infoStartScale = new Vector3(0.8f, 0.8f, 0.8f);
    
    private List<MissionData> currentMissions = new List<MissionData>();

    private void Start()
    {
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        CGBtn.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneName.CG,"..."));

        mission1Btn.onClick.AddListener(() => { OpenInfo(); });
        mission2Btn.onClick.AddListener(() => { OpenInfo(); });
        mission3Btn.onClick.AddListener(() => { OpenInfo(); });
        
        InitUI();
        LoadAcceptedMissions();
        
        missionInfo.localScale = infoStartScale;
    }

    void InitUI()
    {
        if(!SaveManager.Instance.isComplete)
            CGBtn.gameObject.SetActive(false);
        
        missionInfo.gameObject.SetActive(false);
        
        playTimeText.text = "出差次数：" + SaveManager.Instance.playTime.ToString();
        failureTimeText.text = "失败次数：" + SaveManager.Instance.failureTime.ToString();
    }
    
    private void LoadAcceptedMissions()
    {
        currentMissions.Clear();
        
        // 获取所有已接受的任务
        foreach (var mission in LevelManager.Instance.missions)
        {
            if (mission.isMissionAccepted)
            {
                currentMissions.Add(mission);
            }
        }
        
        UpdateMissionButtons();
        UpdateMissionInfo();
    }
    
    private void UpdateMissionButtons()
    {
        mission1Transform.gameObject.SetActive(currentMissions.Count > 0);
        mission2Transform.gameObject.SetActive(currentMissions.Count > 1);
        mission3Transform.gameObject.SetActive(currentMissions.Count > 2);
    }
    
    private void UpdateMissionInfo()
    {
        // 更新任务信息面板的内容
        if (currentMissions.Count > 0)
            UpdateInfoPanel(currentMissions[0]);
        if (currentMissions.Count > 1)
            UpdateInfoPanel(currentMissions[1]);
        if (currentMissions.Count > 2)
            UpdateInfoPanel(currentMissions[2]);
    }
    
    private void UpdateInfoPanel(MissionData mission)
    {
        missionName.text = mission.missionTitle;
        missionDescription.text = mission.missionDescription;

        
        Image iconImage = missionInfo.GetComponentInChildren<Image>();
        if (iconImage != null && mission.missionIcon != null)
        {
            iconImage.sprite = mission.missionIcon;
        }
    }
    
    void OpenInfo()
    {
        if(missionInfo.gameObject.activeSelf) return;
        
        missionInfo.gameObject.SetActive(true);
        CanvasGroup canvasGroup = missionInfo.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = missionInfo.gameObject.AddComponent<CanvasGroup>();
        
        // 重置状态
        canvasGroup.alpha = 0;
        missionInfo.localScale = infoStartScale;
        
        // 同时执行淡入和缩放动画
        Sequence seq = DOTween.Sequence();
        seq.Join(canvasGroup.DOFade(1, infoFadeDuration));
        seq.Join(missionInfo.DOScale(Vector3.one, infoScaleDuration).SetEase(infoScaleEase));
        seq.Play();
    }
}