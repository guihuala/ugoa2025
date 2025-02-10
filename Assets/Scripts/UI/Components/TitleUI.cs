using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


public class TitleUI : MonoBehaviour
{
    public Button Continue;       // 继续游戏按钮
    public Button Load;           // 加载游戏按钮
    public Button New;            // 新游戏按钮
    public Button Exit;           // 退出游戏按钮

    public Button Set;
    public Button Tutorial;
    public Button About;

    public GameObject recordPanel; // 存档面板
    public GameObject setPanel;

    private bool isFirstTimePlay;

    public RectTransform aboutPanelRectTransform;
    public Vector2 targetPosition; // 目标位置，可以根据比例计算
    public Vector2 initialPosition; // 初始位置

    private bool isAboutOpen = false;

    private void Awake()
    { 
        TitleLoadDataUI.OnLoad += LoadRecord;
        
        // 继续游戏，加载最后一次的存档
        Continue.onClick.AddListener(() => LoadRecord(RecordData.Instance.lastID));
        // 打开/关闭存档列表
        Load.onClick.AddListener(OpenRecordPanel);
        // 注册加载存档事件
        // 新游戏（重置数据并开始新游戏）
        New.onClick.AddListener(NewGame);
        // 退出游戏（保存存档）
        Exit.onClick.AddListener(QuitGame);
        
        Set.onClick.AddListener(OpenSetPanel);
        Tutorial.onClick.AddListener(() => { SceneLoader.Instance.LoadScene(SceneName.TutorialScene,"教学...?"); });
        About.onClick.AddListener(ToggleAboutPanel);
    }

    private void OnDestroy()
    {
        // 解绑加载存档事件
        TitleLoadDataUI.OnLoad -= LoadRecord;
    }

    private void Start()
    {
        // 设置 initialPosition 和 targetPosition 基于屏幕比例
        SetAboutPanelPosition();
        
        aboutPanelRectTransform.anchoredPosition = initialPosition;

        // 加载存档数据
        RecordData.Instance.Load();

        // 如果有存档，则激活“继续游戏”和“加载游戏”按钮
        if (RecordData.Instance.lastID != 123)
        {
            Continue.interactable = true;
            Load.interactable = true;

            isFirstTimePlay = false;
        }
        else
        {
            Continue.interactable = false;
            Load.interactable = false;
            
            isFirstTimePlay = true;
        }           
    }

    // 加载指定存档
    void LoadRecord(int i)
    {
        // 加载指定的存档数据
        SaveManager.Instance.Load(i);

        // 如果加载的存档不是最后一次存档，则更新最后一次存档ID
        if (i != RecordData.Instance.lastID)
        {
            RecordData.Instance.lastID = i;
            RecordData.Instance.Save();
        }    

        LevelManager.Instance.LoadLevelUnlocks(i);

        AchievementManager.Instance.LoadAchievements(i);
        
        // 切换到存档所在的场景
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection,"loading...");
    }

    // 打开/关闭存档面板
    void OpenRecordPanel()
    {
        recordPanel.SetActive(!recordPanel.activeSelf);
    }

    void OpenSetPanel()
    {
        setPanel.SetActive(!setPanel.activeSelf);
    }

    // 开始新游戏
    void NewGame()
    {
        // 清除关卡管理器读取的数据，恢复默认值
        LevelManager.Instance.InitLevelUnlocks();
        // 清除成就管理器的数据
        AchievementManager.Instance.InitLockCards();
        
        SaveManager.Instance.ID = RecordData.Instance.GetFirstEmptyRecordIndex();
        
        if(!RecordData.Instance.IsRecordFull())
            SaveManager.Instance.NewRecord();
        else
        {
            ConfirmationPanel confirmationPanel = UIManager.Instance.OpenPanel("ConfirmationPanel") as ConfirmationPanel;

            confirmationPanel.ShowConfirmation("存档已经用完了，确定要开始新的游戏吗？\n 将会覆盖掉第一个存档。", () =>
            {
                SaveManager.Instance.NewRecord();
                SceneLoader.Instance.LoadScene(SceneName.LevelSelection,"loading...");
            });
            return;
        }

        if (isFirstTimePlay)
        {
            SceneLoader.Instance.LoadScene(SceneName.TutorialScene,"Step Or Sink...？");
            return;
        }
        
        // 切换到默认场景
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection,"loading...");
    }

    // 退出游戏
    void QuitGame()
    {     
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();                          
        #endif
    }
    
    void ToggleAboutPanel()
    {
        if (isAboutOpen)
        {
            aboutPanelRectTransform.DOKill();
            aboutPanelRectTransform.DOLocalMove(initialPosition, 0.5f);
        }
        else
        {
            aboutPanelRectTransform.localPosition = initialPosition;
            aboutPanelRectTransform.DOLocalMove(targetPosition, 0.5f);
        }
        isAboutOpen = !isAboutOpen;
    }
    
    private void SetAboutPanelPosition()
    {
        initialPosition = new Vector2(Screen.width * -0.1f, Screen.height * 1.0f);
        targetPosition = new Vector2(Screen.width * -0.1f, Screen.height * 0.2f);
    }
}
