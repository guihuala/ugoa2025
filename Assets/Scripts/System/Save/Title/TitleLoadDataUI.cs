using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// 具备加载功能的存档面板，用于标题场景
public class TitleLoadDataUI : MonoBehaviour
{
    public Transform grid; // 存档列表的容器
    public GameObject recordPrefab; // 存档项预制件
    public Button closeBtn;

    [Header("存档详情")] public GameObject detail; // 存档详情面板
    public Text gameTime; // 游戏时间
    public Text sceneName; // 当前场景

    // 加载存档时触发的事件
    public static System.Action<int> OnLoad;


    private void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
        detail.SetActive(false);

        // 绑定事件
        RecordUI.OnLeftClick += LeftClickGrid;
        RecordUI.OnEnter += ShowDetails;
        
        UpdateInfo();
    }

    private void OnDestroy()
    {
        // 解绑事件
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnEnter -= ShowDetails;
    }

    public void UpdateInfo()
    {
        // 初始化存档列表
        for (int i = 0; i < RecordData.recordNum; i++)
        {
            GameObject obj = Instantiate(recordPrefab, grid);
            // 设置名称
            obj.name = (i + 1).ToString();
            obj.GetComponent<RecordUI>().SetID(i + 1);
            // 如果存档存在，则设置存档名称
            if (RecordData.Instance.recordName[i] != "")
                obj.GetComponent<RecordUI>().SetName(i);
            
            Debug.Log(RecordData.Instance.recordName[i]);
        }
    }

    // 显示存档详情（鼠标进入事件）
    void ShowDetails(int i)
    {
        // 检查存档索引合法性
        if (i < 0 || i >= RecordData.recordNum)
        {
            detail.SetActive(false);
            return;
        }

        // 获取存档数据
        var data = SaveManager.Instance.ReadForShow(i);

        if (data == null || RecordData.Instance.recordName[i] == "")
        {
            // 存档为空，显示提示信息
            gameTime.text = "游戏时间：无";
            sceneName.text = "游戏进度：无存档";
            detail.SetActive(true); // 显示详情面板
            detail.transform.localScale = Vector3.zero; // 初始为0大小
            detail.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); // 缩放进入
            return;
        }

        // 如果存档不为空，正常更新存档详情
        gameTime.text = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"游戏进度  {LevelManager.Instance.GetLastUnlockedLevel(data.levelUnlocks)}";

        // 显示详情面板
        detail.SetActive(true);
        detail.transform.localScale = Vector3.zero; // 初始为0大小
        detail.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); // 缩放进入
    }


    // 加载选中的存档
    void LeftClickGrid(int gridID)
    {
        // 如果存档为空，则不处理
        if (RecordData.Instance.recordName[gridID] == "")
            return;
        else
        {
            // 触发加载事件
            if (OnLoad != null)
                OnLoad(gridID);
        }
    }
}