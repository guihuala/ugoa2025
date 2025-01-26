using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RecordPanel : BasePanel
{
    public Transform grid;               // 存档列表的容器
    public GameObject recordPrefab;      // 存档项预制件

    [Header("按钮")]
    public Button saveBtn;
    public Button loadBtn;
    public Button closeBtn;
    
    [ColorUsage(true)]
    public Color oriColor;              // 按钮初始颜色

    [Header("存档详情")]   
    public GameObject detail;           // 存档详情面板
    public Image screenShot;            // 存档截图
    public Text gameTime;               // 游戏时间
    public Text sceneName;              // 当前场景

    // Key：存档文件名，Value：存档编号
    Dictionary<string, int> RecordInGrid = new Dictionary<string, int>();
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
        RecordUI.OnRightClick += RightClickGrid;
        RecordUI.OnEnter += ShowDetails;
        saveBtn.onClick.AddListener(() => SaveOrLoad());
        loadBtn.onClick.AddListener(() => SaveOrLoad(false));
        closeBtn.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        #endregion

        // 初始化时间
        TIMEMGR.SetOriTime();
    }

    private void OnDestroy()
    {
        RecordUI.OnLeftClick -= LeftClickGrid;
        RecordUI.OnRightClick -= RightClickGrid;
        RecordUI.OnEnter -= ShowDetails;
    }

    private void Update()
    {
        TIMEMGR.SetCurTime();
    }

    // 显示存档详情（鼠标进入事件）
    void ShowDetails(int i)
    {
        // 获取存档数据并更新显示
        var data = SaveManager.Instance.ReadForShow(i);
        if (data == null)
            return;
        
        gameTime.text = $"游戏时间  {TIMEMGR.GetFormatTime((int)data.gameTime)}";
        sceneName.text = $"当前场景  {data.scensName}";
        screenShot.sprite = SAVE.LoadShot(i);

        // 显示详情面板
        detail.SetActive(true);
    }
    

    // 切换存档/加载模式
    void SaveOrLoad(bool OnSave = true)
    {
        // 更新模式
        isSave = OnSave;
        isLoad = !OnSave;
        // 更新按钮颜色
        saveBtn.GetComponent<Image>().color = isSave ? Color.white : oriColor;
        loadBtn.GetComponent<Image>().color = isLoad ? Color.white : oriColor;
    }

    // 左键点击事件
    void LeftClickGrid(int ID)
    {
        // 存档
        if (isSave)
        {           
            NewRecord(ID);
        }
        // 加载
        else if (isLoad)
        {
            // 如果为空存档则不处理
            if (RecordData.Instance.recordName[ID] == "")           
                return;           
            else
            {
                // 加载存档数据
                SaveManager.Instance.Load(ID);    
                // 更新当前存档ID并保存到存档数据
                RecordData.Instance.lastID = ID;
                RecordData.Instance.Save();

                // 切换场景并更新时间
                if (SceneManager.GetActiveScene().name != SaveManager.Instance.scensName.ToString())
                {
                    SceneLoader.Instance.LoadScene(SaveManager.Instance.scensName,"...");
                }
                TIMEMGR.SetOriTime();
            }
        }
    }

    // 右键点击事件（删除存档）
    void RightClickGrid(int gridID)
    {
        if (RecordData.Instance.recordName[gridID] == "")        
            return;
        
        // 删除存档
        DeleteRecord(gridID, false);
    }

    // 创建新存档
    void NewRecord(int i, string end = ".save")
    {
        // 如果原位置有存档则删除
        if (RecordData.Instance.recordName[i] != "")
        {
            DeleteRecord(i);
        }

        // 创建新存档
        RecordData.Instance.recordName[i] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        RecordData.Instance.lastID = i;
        RecordData.Instance.Save();
        SaveManager.Instance.Save(i);
        RecordInGrid.Add(RecordData.Instance.recordName[i], i);
        grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
        SAVE.CameraCapture(i, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
        ShowDetails(i);
    }

    // 删除存档
    void DeleteRecord(int i, bool isCover = true)
    {
        // 删除存档文件
        SaveManager.Instance.Delete(i);
        RecordInGrid.Remove(RecordData.Instance.recordName[i]);

        if (!isCover)
        {           
            // 清空存档数据
            RecordData.Instance.recordName[i] = "";
            grid.GetChild(i).GetComponent<RecordUI>().SetName(i);
            SAVE.DeleteShot(i);
            detail.SetActive(false);
        }
    }
}
