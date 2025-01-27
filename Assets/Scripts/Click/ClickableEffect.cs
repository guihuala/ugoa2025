using UnityEngine;
using DG.Tweening;

public class ClickableEffect : MonoBehaviour, IClickable
{
    [Header("Click Effect Settings")]
    [SerializeField] private float timeScaleSlow = 0.2f; // 时间减缓比例
    private bool isEffectActive = false; // 是否激活效果
    private GameObject clickUI; // 生成的 UI 对象
    private CanvasGroup uiCanvasGroup; // 用于控制 UI 透明度

    private void Awake()
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

    public void OnClick()
    {
        // 切换效果状态
        isEffectActive = !isEffectActive;

        // 触发事件
        EVENTMGR.TriggerTimeScaleChange(isEffectActive ? timeScaleSlow : 1.0f);
        EVENTMGR.TriggerClickCharacter(isEffectActive);
    }

    public void ShowUIWithAnimation()
    {
        clickUI.SetActive(true);
        uiCanvasGroup.alpha = 0;
        uiCanvasGroup.DOFade(1, 0.1f).SetEase(Ease.InOutQuad); // 渐变动画
        clickUI.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.OutBack); // 缩放动画
    }

    public void HideUIWithAnimation()
    {
        uiCanvasGroup.DOFade(0, 0.1f).SetEase(Ease.InOutQuad).OnComplete(() =>
        {
            clickUI.SetActive(false);
        });
        clickUI.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.InBack); // 缩放回零
    }
}