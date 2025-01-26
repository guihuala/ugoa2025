using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordPanel : BasePanel
{
    public Transform grid;               // 存档列表的容器
    public GameObject recordPrefab;      // 存档项预制件

    [Header("按钮")]
    public Button closeBtn;
    public Button saveBtn;
    public Button loadBtn;
    public Button deleteBtn;

    [Header("存档详情")]   
    public GameObject detail;           // 存档详情面板
    public Image screenShot;            // 存档截图
    public Text gameTime;               // 游戏时间
    public Text sceneName;              // 当前场景

    // Key：存档文件名，Value：存档编号
    Dictionary<string, int> RecordInGrid = new Dictionary<string, int>();
    private int currenSelectIndex;    // 当前选择的记录编号
    bool isSave = false;     // 当前是否处于存档模式
    bool isLoad = false;     // 当前是否处于加载模式

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
            {
                obj.GetComponent<RecordUI>().SetName(i);               
                // 添加到字典中
                RecordInGrid.Add(RecordData.Instance.recordName[i], i);
            }            
        }

        #region 按钮绑定
        RecordUI.OnLeftClick += LeftClickGrid;     
        
        closeBtn.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        saveBtn.onClick.AddListener(() => OnSaveBtnClick());
        loadBtn.onClick.AddListener(() => OnLoadBtnClick());
        deleteBtn.onClick.AddListener(() => OnDeleteBtnClick());
        #endregion
        
        detail.SetActive(false);

        // 初始化时间
        TIMEMGR.SetOriTime();
    }

    private void OnDestroy()
    {
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnEnter -= ShowDetails;
    }

    private void Update()
    {
        TIMEMGR.SetCurTime();
    }

    // 显示存档详情（鼠标进入事件）
    void ShowDetails(int i)
    {
        // 检查存档索引合法性
        if (i < 0 || i >= RecordData.recordNum)
        {
            Debug.LogWarning("非法存档索引！");
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
            return;
        }

        // 如果存档不为空，正常更新存档详情
        gameTime.text = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"当前场景  {data.scensName}";
        screenShot.sprite = SAVE.LoadShot(i);

        // 显示详情面板
        detail.SetActive(true);
    }


    // 保存按钮点击事件
    void OnSaveBtnClick()
    {
        if (currenSelectIndex < 0 || currenSelectIndex >= RecordData.recordNum)
        {
            Debug.LogWarning("未选择任何存档位置！");
            return;
        }
        NewRecord(currenSelectIndex);
        ShowDetails(currenSelectIndex);
    }

    // 加载按钮点击事件
    void OnLoadBtnClick()
    {
        LoadRecord(currenSelectIndex);
    }
    
    // 删除按钮点击事件
    void OnDeleteBtnClick()
    {
        // 删除存档
        DeleteRecord(currenSelectIndex, false);
    }

    // 左键点击事件
    void LeftClickGrid(int ID)
    {
        currenSelectIndex = ID;
    }

    // 创建新存档
    void NewRecord(int ID, string end = ".save")
    {
        // 如果原位置有存档则删除
        if (RecordData.Instance.recordName[ID] != "")
        {
            DeleteRecord(ID);
        }

        // 创建新存档
        RecordData.Instance.recordName[ID] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        RecordData.Instance.lastID = ID;
        RecordData.Instance.Save();
        
        SaveManager.Instance.Save(ID);
        RecordInGrid.Add(RecordData.Instance.recordName[ID], ID);
        grid.GetChild(ID).GetComponent<RecordUI>().SetName(ID);
        SAVE.CameraCapture(ID, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
        ShowDetails(ID);
    }

    void LoadRecord(int ID)
    {
        // 如果为空存档则不处理
        if (RecordData.Instance.recordName[ID] == "")
            return;

        // 加载存档数据
        SaveManager.Instance.Load(ID);
        // 更新当前存档ID并保存到存档数据
        RecordData.Instance.lastID = ID;
        RecordData.Instance.Save();

        // 切换场景并更新时间
        if (SceneManager.GetActiveScene().name != SaveManager.Instance.scensName.ToString())
        {
            SceneLoader.Instance.LoadScene(SaveManager.Instance.scensName, "...");
        }

        TIMEMGR.SetOriTime();
    }
    
    // 删除存档
    void DeleteRecord(int i, bool isCover = true)
    {
        if (i < 0 || i >= RecordData.recordNum || RecordData.Instance.recordName[i] == "")
        {
            Debug.LogWarning("删除存档失败：非法的存档索引！");
            return;
        }
        SaveManager.Instance.Delete(i);
        RecordData.Instance.Delete();
        RecordInGrid.Remove(RecordData.Instance.recordName[i]);

        if (!isCover)
        {
            RecordData.Instance.recordName[i] = "";
            grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
            SAVE.DeleteShot(i);
            detail.SetActive(false);
        }
    }
}
