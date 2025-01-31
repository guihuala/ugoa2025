using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

// 具备加载功能的存档面板，用于标题场景
public class TitleLoadDataUI : MonoBehaviour
{
    public Transform grid;               // 存档列表的容器
    public GameObject recordPrefab;      // 存档项预制件

    [Header("存档详情")]
    public GameObject detail;           // 存档详情面板
    public Image screenShot;            // 存档截图
    public Text gameTime;               // 游戏时间
    public Text sceneName;              // 当前场景

    // 加载存档时触发的事件
    public static System.Action<int> OnLoad;

    private void Start()
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
        }

        // 绑定事件
        RecordUI.OnLeftClick += LeftClickGrid;
        RecordUI.OnEnter += ShowDetails;
        RecordUI.OnExit += HideDetails;
        
        detail.SetActive(false);
    }

    private void OnDestroy()
    {
        // 解绑事件
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnEnter -= ShowDetails;
        RecordUI.OnExit -= HideDetails;
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
            sceneName.text = "当前场景：无存档";
            screenShot.sprite = null; // 清空截图显示
            detail.SetActive(true);  // 显示详情面板
            detail.transform.localScale = Vector3.zero; // 初始为0大小
            detail.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); // 缩放进入
            return;
        }

        // 如果存档不为空，正常更新存档详情
        gameTime.text = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"当前场景  {data.scensName}";
        screenShot.sprite = SAVE.LoadShot(i);

        // 显示详情面板
        detail.SetActive(true);
        detail.transform.localScale = Vector3.zero; // 初始为0大小
        detail.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack); // 缩放进入
    }


    // 隐藏存档详情（鼠标离开存档项时触发）
    void HideDetails()
    {
        detail.SetActive(false);
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
