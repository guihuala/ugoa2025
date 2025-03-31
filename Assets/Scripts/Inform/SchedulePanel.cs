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
    
    [Header("游戏次数统计")]
    public Text playTimeText;
    public Text failureTimeText;
    
    [Header("新任务提示")]
    public GameObject newMissionDot; // 红点提示
    
    private List<MissionData> currentMissions = new List<MissionData>();
    
    private void Start()
    {
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        CGBtn.onClick.AddListener(() => SceneLoader.Instance.LoadScene(SceneName.CG,"..."));

        mission1Btn.onClick.AddListener(() => { OpenInfo(mission1Info); });
        mission2Btn.onClick.AddListener(() => { OpenInfo(mission2Info); });
        mission3Btn.onClick.AddListener(() => { OpenInfo(mission3Info); });
        
        InitUI();
        
        // 添加新任务
    }

    void InitUI()
    {
        if(!SaveManager.Instance.isComplete)
            CGBtn.gameObject.SetActive(false);
        
        mission1Info.gameObject.SetActive(false);
        mission2Info.gameObject.SetActive(false);
        mission3Info.gameObject.SetActive(false);
        
        playTimeText.text = "出差次数：" + SaveManager.Instance.playTime.ToString();
        failureTimeText.text = "失败次数：" + SaveManager.Instance.failureTime.ToString();
    }
    
    // 添加新任务方法
    public void AddNewMission(MissionData mission)
    {
        // 已接受且不允许重复
        if (!currentMissions.Contains(mission) && mission.isMissionAccepted)
        {
            currentMissions.Add(mission);
            UpdateMissionButtons();
            newMissionDot.SetActive(true);
        }
    }
    
    // 更新任务按钮状态
    private void UpdateMissionButtons()
    {
        // 根据currentMissions更新按钮状态
        mission1Btn.gameObject.SetActive(currentMissions.Count > 0);
        mission2Btn.gameObject.SetActive(currentMissions.Count > 1);
        mission3Btn.gameObject.SetActive(currentMissions.Count > 2);
        
        // 更新按钮文本等信息
        if (currentMissions.Count > 0)
            mission1Btn.GetComponentInChildren<Text>().text = currentMissions[0].missionID;
    }
    
    // 当查看任务信息后调用
    public void OnMissionViewed()
    {
        newMissionDot.SetActive(false);
    }
    
    void OpenInfo(Transform info)
    {
        info.gameObject.SetActive(true);
        OnMissionViewed(); // 查看后取消红点
    }
}
