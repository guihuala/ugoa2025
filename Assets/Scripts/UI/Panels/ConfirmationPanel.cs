using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ConfirmationPanel : BasePanel
{
    [Header("UI元素")]
    public Text messageText; // 显示确认信息的文本
    public Button confirmButton; // 确认按钮
    public Button cancelButton; // 取消按钮

    private System.Action onConfirm; // 确认回调

    public override void OpenPanel(string name)
    {
        panelName = name;
        
        gameObject.SetActive(true);
    }
    
    public override void ClosePanel()
    {
        hasRemoved = true;

        Destroy(gameObject);
    }

    protected override void Awake()
    {
        base.Awake();
        
        if (confirmButton != null)
        {
            confirmButton.onClick.AddListener(OnConfirmButtonClicked);
        }

        if (cancelButton != null)
        {
            cancelButton.onClick.AddListener(OnCancelButtonClicked);
        }
    }

    /// <summary>
    /// 初始化并显示确认面板
    /// </summary>
    /// <param name="message">显示的确认信息</param>
    /// <param name="onConfirmAction">点击确认时执行的回调</param>
    /// <param name="onCancelAction">点击取消时执行的回调</param>
    public void ShowConfirmation(string message, System.Action onConfirmAction)
    {
        // 设置面板的提示信息
        if (messageText != null)
        {
            messageText.text = message;
        }

        // 绑定回调方法
        onConfirm = onConfirmAction;
    }
    
    private void OnConfirmButtonClicked()
    {
        onConfirm?.Invoke();
    }
    
    private void OnCancelButtonClicked()
    {
        UIManager.Instance.ClosePanel(panelName);
    }
}
