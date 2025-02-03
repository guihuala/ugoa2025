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

    protected override void Awake()
    {
        base.Awake();
        
        tab1Button.onClick.AddListener(() => ShowTabContent(1));
        tab2Button.onClick.AddListener(() => ShowTabContent(2));
        tab3Button.onClick.AddListener(() => ShowTabContent(3));

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
        if (currentContent != null)
        {
            // 如果当前页签有内容，渐隐
            currentContent.GetComponent<CanvasGroup>().DOFade(0, 0.3f).OnComplete(() =>
            {
                currentContent.SetActive(false); // 隐藏当前内容
            }).SetUpdate(true);
        }

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
        currentContent.GetComponent<CanvasGroup>().alpha = 0;

        currentContent.GetComponent<CanvasGroup>().DOFade(1, 0.3f).SetUpdate(true);
    }
}
