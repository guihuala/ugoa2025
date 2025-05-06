using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;

public class ClickableEffect : MonoBehaviour, IClickable
{
    [Header("点击设置")]
    [SerializeField] private float timeScaleSlow = 0.2f;
    [SerializeField] private bool showHintUI = true; // 是否显示提示UI

    private GameObject clickUI; // 生成的 UI 对象
    private CanvasGroup uiCanvasGroup; // 用于控制 UI 透明度
    
    private bool isUIOpen = false;
    public bool isActive = true; // 是否可点击

    private void Awake()
    {
        if (showHintUI)
        {
            // 动态生成 UI
            clickUI = Instantiate(Resources.Load<GameObject>("UIcomponents/ClickUI"));
            clickUI.transform.SetParent(transform);
            clickUI.transform.localPosition = new Vector3(0, 0.5f, 0);
            clickUI.SetActive(false);

            uiCanvasGroup = clickUI.GetComponent<CanvasGroup>();
            if (uiCanvasGroup == null)
            {
                uiCanvasGroup = clickUI.AddComponent<CanvasGroup>();
            }
        }
    }

    public void OnClick()
    {
        // 如果对象未激活，直接返回
        if (!isActive) return;

        // 如果鼠标指针位于 UI 上，则不执行点击检测
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        if (isUIOpen) // ui存在时，再次点击玩家，关闭画布
        {
            EVENTMGR.TriggerClickPlayer(false);
            EVENTMGR.TriggerTimeScaleChange(1.0f);
            if (showHintUI) HideUIWithAnimation();
            return;
        }

        // 触发点击事件（仅在激活时）
        EVENTMGR.TriggerClickPlayer(true);
        EVENTMGR.TriggerTimeScaleChange(timeScaleSlow);
        if (showHintUI) ShowUIWithAnimation();
    }

    public void ShowUIWithAnimation()
    {
        if (!showHintUI || clickUI == null) return;

        clickUI.SetActive(true);
        uiCanvasGroup.alpha = 0;
        uiCanvasGroup.DOFade(1, 0.1f).SetEase(Ease.InOutQuad); // 渐变动画
        clickUI.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack); // 缩放动画
        
        isUIOpen = true;
    }

    public void HideUIWithAnimation()
    {
        if (!showHintUI || clickUI == null) return;

        uiCanvasGroup.DOFade(0, 0.1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            clickUI.SetActive(false);
        });
        clickUI.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack); // 缩放回零
        
        isUIOpen = false;
    }
    
    public void Activate()
    {
        isActive = true;
    }
    
    public void Deactivate()
    {
        isActive = false;
        if (isUIOpen && showHintUI) HideUIWithAnimation(); // 关闭UI
    }
}