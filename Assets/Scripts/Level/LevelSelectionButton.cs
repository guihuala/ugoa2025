using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LevelSelectionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool unlocked;

    [SerializeField] private GameObject themeUnlockImg;
    [SerializeField] private GameObject themeLockImg;
    [SerializeField] private GameObject levelLockImg;
    
    [SerializeField] private string levelName;
    [SerializeField] private SceneName sceneName;

    [Header("Tooltip设置")]
    [SerializeField] private GameObject tooltipPrefab;
    [SerializeField] private Vector3 tooltipOffset = new Vector3(0, 0.01f, 0.01f); // 世界空间偏移量
    [SerializeField] private string lockedTooltipText = "关卡未解锁";
    [SerializeField] private string specialConditionText = "需要特殊物品解锁";

    private GameObject currentTooltip;
    private LevelData currentLevel;
    private Canvas parentCanvas; // 父级Canvas（用于判断渲染模式）

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() => PressSelection(sceneName));
        parentCanvas = GetComponentInParent<Canvas>(); // 获取父Canvas
    }

    private void Start()
    {
        UpdateLevelStatus();
        UpdateLevelImage();
    }

    private void UpdateLevelStatus()
    {
        currentLevel = LevelManager.Instance.levels.Find(l => l.name == levelName);
        if (currentLevel != null)
        {
            if (currentLevel.requiresItems)
            {
                LevelManager.Instance.UnlockSpecialLevel(currentLevel);
            }
            unlocked = currentLevel.isUnlocked;
        }
    }

    private void UpdateLevelImage()
    {
        levelLockImg.SetActive(!unlocked);
        SetLockImg(unlocked);
        SetUnlockImg(unlocked);
    }

    void SetLockImg(bool isUnlocked)
    {
        if (themeLockImg != null)
            themeLockImg.SetActive(!isUnlocked);
    }

    void SetUnlockImg(bool isUnlocked)
    {
        if (themeUnlockImg != null)
            themeUnlockImg.SetActive(isUnlocked);
    }

    public void PressSelection(SceneName _LevelName)
    {
        if (unlocked)
        {
            SaveManager.Instance.playTime++;
            SceneLoader.Instance.LoadScene(_LevelName, "...");
        }
    }

    // 鼠标悬停时显示Tooltip
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!unlocked && tooltipPrefab != null)
        {
            currentTooltip = Instantiate(tooltipPrefab);

            // 设置Tooltip位置
            if (parentCanvas.renderMode == RenderMode.WorldSpace)
            {
                currentTooltip.transform.rotation = transform.rotation; // 保持与按钮相同的旋转
                currentTooltip.transform.SetParent(transform);
                currentTooltip.transform.localPosition = new Vector3(0, 0, 0) + tooltipOffset;
            }
            
            // 设置Tooltip文本
            var tooltipText = currentTooltip.GetComponentInChildren<Text>();
            if (tooltipText != null)
            {
                tooltipText.text = currentLevel.requiresItems
                    ? specialConditionText
                    : lockedTooltipText;
            }
        }
    }

    // 鼠标离开时销毁Tooltip
    public void OnPointerExit(PointerEventData eventData)
    {
        if (currentTooltip != null)
        {
            Destroy(currentTooltip);
            currentTooltip = null;
        }
    }

    private void OnDestroy()
    {
        if (currentTooltip != null)
            Destroy(currentTooltip);
    }
}