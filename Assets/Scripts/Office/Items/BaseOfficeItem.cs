using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseOfficeItem : MonoBehaviour
{
    private Vector3 originalScale = new Vector3(1, 1, 1);
    private PlayerInteractManager playerInteractManager;
    
    private GameObject clickUI; // 物体名称画布
    private CanvasGroup uiCanvasGroup; // 用于控制 UI 透明度
    private TextMesh itemNameText; // 物体名称文本

    [Header("物品名称显示设置")]
    [SerializeField] private string itemName;
    [SerializeField] private float canvasFadeDuration = 0.3f; // 画布淡入淡出时间
    private float nameFloatHeight = 0.2f; // 名称浮动高度
    private float nameFloatSpeed = 2f; // 名称浮动速度
    private float showScaleAmount = 1.1f; // 显示时的缩放比例
    [SerializeField] private Ease showEase = Ease.OutBack; // 显示动画缓动类型
    [SerializeField] private Ease hideEase = Ease.InBack; // 隐藏动画缓动类型

    private Vector3 uiOriginalPosition; // UI原始位置
    private bool isNameVisible = false; // 名称是否可见
    private Sequence floatSequence; // 浮动动画序列

    protected virtual void Start()
    {
        HideHighlight();
        playerInteractManager = FindObjectOfType<PlayerInteractManager>();
        
        // 动态生成 UI
        clickUI = Instantiate(Resources.Load<GameObject>("UIcomponents/TipsUI"));
        clickUI.transform.SetParent(transform);
        clickUI.transform.localPosition = new Vector3(0, 0.2f, 0);
        uiOriginalPosition = clickUI.transform.localPosition;
        clickUI.SetActive(false);
        
        itemNameText = clickUI.GetComponentInChildren<TextMesh>();
        if (itemNameText != null)
        {
            itemNameText.text = itemName;
        }
        
        uiCanvasGroup = clickUI.GetComponent<CanvasGroup>();
        if (uiCanvasGroup == null)
        {
            uiCanvasGroup = clickUI.AddComponent<CanvasGroup>();
        }
        
        // 初始化时设置透明度为0
        uiCanvasGroup.alpha = 0;
        clickUI.transform.localScale = Vector3.zero;
    }

    private void Update()
    {
        if (!playerInteractManager.CheckItem(this))
        {
            return;
        }

        CheckHover();
        CheckClick();
    }

    // 显示物体名称
    public void ShowItemName()
    {
        if (clickUI == null || isNameVisible) return;

        isNameVisible = true;
        
        // 停止所有动画以防冲突
        uiCanvasGroup.DOKill();
        clickUI.transform.DOKill();
        if (floatSequence != null && floatSequence.IsActive()) floatSequence.Kill();

        // 激活UI
        clickUI.SetActive(true);
        
        // 动画序列
        Sequence showSequence = DOTween.Sequence();
        
        // 淡入效果
        showSequence.Join(uiCanvasGroup.DOFade(1, canvasFadeDuration));
        
        // 缩放效果 - 弹跳出现
        showSequence.Join(clickUI.transform.DOScale(Vector3.one * showScaleAmount, canvasFadeDuration * 0.5f)
            .SetEase(showEase));
        showSequence.Append(clickUI.transform.DOScale(Vector3.one, canvasFadeDuration * 0.5f)
            .SetEase(showEase));
        
        // 浮动效果
        floatSequence = DOTween.Sequence();
        floatSequence.Append(clickUI.transform.DOLocalMoveY(uiOriginalPosition.y + nameFloatHeight, nameFloatSpeed)
            .SetEase(Ease.InOutSine));
        floatSequence.Append(clickUI.transform.DOLocalMoveY(uiOriginalPosition.y, nameFloatSpeed)
            .SetEase(Ease.InOutSine));
        floatSequence.SetLoops(-1, LoopType.Yoyo);
    }

    // 隐藏物体名称
    public void HideItemName()
    {
        if (clickUI == null || !isNameVisible) return;

        isNameVisible = false;
        
        // 停止所有动画
        uiCanvasGroup.DOKill();
        clickUI.transform.DOKill();
        if (floatSequence != null && floatSequence.IsActive()) floatSequence.Kill();

        // 动画序列
        Sequence hideSequence = DOTween.Sequence();
        
        // 淡出效果
        hideSequence.Join(uiCanvasGroup.DOFade(0, canvasFadeDuration));
        
        // 缩放效果 - 缩小消失
        hideSequence.Join(clickUI.transform.DOScale(Vector3.zero, canvasFadeDuration)
            .SetEase(hideEase));
        
        // 重置位置
        hideSequence.Join(clickUI.transform.DOLocalMove(uiOriginalPosition, canvasFadeDuration));
        
        hideSequence.OnComplete(() => {
            clickUI.SetActive(false);
            clickUI.transform.localScale = Vector3.zero;
        });
    }
    
    private void CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                ShowHighlight(1.3f);
            }
            else
            {
                ShowHighlight(1.1f);
            }
        }
    }

    public void ShowHighlight(float scaleFactor)
    {
        if (playerInteractManager.CheckItem(this) && transform.localScale != originalScale * scaleFactor)
        {
            transform.DOScale(originalScale * scaleFactor, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
            
            ShowItemName();
        }
    }

    public void HideHighlight()
    {
        transform.DOScale(new Vector3(1,1,1), 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
        HideItemName();
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    Apply();
                }
            }
        }
    }

    protected virtual void Apply() 
    {
        // 应用逻辑
    }

    private void OnDestroy()
    {
        // 清理动画
        if (floatSequence != null && floatSequence.IsActive()) floatSequence.Kill();
    }
}