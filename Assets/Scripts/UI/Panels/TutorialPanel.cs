using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : BasePanel
{
    [Header("Tab按钮")]
    [SerializeField] private Button tab1Button;
    [SerializeField] private Button tab2Button;
    [SerializeField] private Button tab3Button;
    [SerializeField] private Button tab4Button;

    [Header("教程内容")]
    [SerializeField] private GameObject tab1Content;
    [SerializeField] private GameObject tab2Content;
    [SerializeField] private GameObject tab3Content;
    [SerializeField] private GameObject tab4Content;
    
    [Header("关闭按钮")]
    public Button closeBtn;

    private GameObject currentContent;
    private int currentIndex;
    private Button currentTabButton;  // 当前选中的tab按钮
    private Color normalTabColor = Color.white; // 默认tab颜色
    public Color selectedTabColor; // 选中tab颜色

    protected override void Awake()
    {
        base.Awake();
        
        tab1Button.onClick.AddListener(() => ShowTabContent(1));
        tab2Button.onClick.AddListener(() => ShowTabContent(2));
        tab3Button.onClick.AddListener(() => ShowTabContent(3));
        tab4Button.onClick.AddListener(() => ShowTabContent(4));  // 添加对 tab4Button 的监听

        closeBtn.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));

        // 默认显示第一页的内容
        ShowTabContent(1);
    }
    
    /// <summary>
    /// 显示指定页签的内容
    /// </summary>
    /// <param name="tabIndex">页签索引，1 表示第一页，2 表示第二页，3 表示第三页</param>
    private void ShowTabContent(int tabIndex)
    {
        if(currentIndex == tabIndex)
            return;
        
        if (currentContent != null)
        {
            currentContent.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() =>
            {
                currentContent.SetActive(false);
            }).SetUpdate(true);
        }

        UpdateTabButtonColor(tabIndex);

        switch (tabIndex)
        {
            case 1:
                currentContent = tab1Content;
                break;
            case 2:
                currentContent = tab2Content;
                break;
            case 3:
                currentContent = tab3Content;
                break;
            case 4:
                currentContent = tab4Content;
                break;
        }
        
        currentContent.SetActive(true);
        CanvasGroup canvasGroup = currentContent.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0;
        
        canvasGroup.DOFade(1, 0.3f).OnComplete(() =>
        {
            currentContent.SetActive(true);
        }).SetUpdate(true);
    }

    /// <summary>
    /// 更新按钮颜色
    /// </summary>
    /// <param name="tabIndex">当前选中的tab的索引</param>
    private void UpdateTabButtonColor(int tabIndex)
    {
        // 恢复所有tab按钮的默认颜色
        tab1Button.GetComponent<Image>().color = normalTabColor;
        tab2Button.GetComponent<Image>().color = normalTabColor;
        tab3Button.GetComponent<Image>().color = normalTabColor;
        tab4Button.GetComponent<Image>().color = normalTabColor;

        // 改变选中tab按钮的颜色
        switch (tabIndex)
        {
            case 1:
                tab1Button.GetComponent<Image>().color = selectedTabColor;
                break;
            case 2:
                tab2Button.GetComponent<Image>().color = selectedTabColor;
                break;
            case 3:
                tab3Button.GetComponent<Image>().color = selectedTabColor;
                break;
            case 4:
                tab4Button.GetComponent<Image>().color = selectedTabColor;
                break;
        }
        currentIndex = tabIndex;
    }
}
