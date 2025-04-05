using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class GetNewItemUI : MonoBehaviour
{
    [Header("UI组件配置")]
    public CanvasGroup panelCanvasGroup;
    public Image itemIcon;
    public Text itemNameText;
    public Text itemDescriptionText;
    
    [Header("动画设置")]
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.3f;
    public float displayDuration = 3f;
    
    private bool isShowing = false;
    private Coroutine autoHideCoroutine;

    private void Awake()
    {
        // 初始隐藏面板
        panelCanvasGroup.alpha = 0;
        panelCanvasGroup.blocksRaycasts = false;
        panelCanvasGroup.interactable = false;
    }

    private void OnEnable()
    {
        // 订阅事件
        EVENTMGR.OnUnlockItem += OnItemUnlocked;
    }

    private void OnDisable()
    {
        // 取消订阅事件
        EVENTMGR.OnUnlockItem -= OnItemUnlocked;
    }

    private void Update()
    {
        // 如果UI正在显示且点击了鼠标任意键
        if (isShowing && Input.GetMouseButtonDown(0))
        {
            HidePanel();
        }
    }

    private void OnItemUnlocked(ItemData itemData)
    {
        // 更新UI内容
        itemIcon.sprite = itemData.icon;
        itemNameText.text = itemData.itemName;
        itemDescriptionText.text = itemData.description;
        
        // 显示面板
        ShowPanel();
    }

    private void ShowPanel()
    {
        if (isShowing) return;
        
        isShowing = true;
        
        // 停止之前的自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
        }
        
        // 显示面板动画
        panelCanvasGroup.blocksRaycasts = true;
        panelCanvasGroup.interactable = true;
        
        panelCanvasGroup.DOKill();
        panelCanvasGroup.DOFade(1, fadeInDuration)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => {
                // 开始自动隐藏计时
                autoHideCoroutine = StartCoroutine(AutoHideAfterDelay());
            });
    }

    private void HidePanel()
    {
        if (!isShowing) return;
        
        // 停止自动隐藏协程
        if (autoHideCoroutine != null)
        {
            StopCoroutine(autoHideCoroutine);
            autoHideCoroutine = null;
        }
        
        // 隐藏面板动画
        panelCanvasGroup.blocksRaycasts = false;
        panelCanvasGroup.interactable = false;
        
        panelCanvasGroup.DOKill();
        panelCanvasGroup.DOFade(0, fadeOutDuration)
            .SetEase(Ease.InQuad)
            .OnComplete(() => {
                isShowing = false;
            });
    }

    private IEnumerator AutoHideAfterDelay()
    {
        yield return new WaitForSeconds(displayDuration);
        HidePanel();
    }
}