using DG.Tweening;
using UnityEngine;

/// <summary>
/// UI面板的基类
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class BasePanel : MonoBehaviour
{
    protected bool hasRemoved = false; // 标记面板是否已被移除
    protected string panelName; // 面板名称
    protected CanvasGroup canvasGroup; // 用于管理透明度和交互

    protected virtual void Awake()
    {
        // 获取 CanvasGroup 组件
        canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 打开面板
    /// </summary>
    /// <param name="name">面板名称</param>
    public virtual void OpenPanel(string name)
    {
        if (hasRemoved) return;  // 如果面板已经被移除，避免重复执行

        panelName = name;
        
        // 设置初始透明度为 0
        canvasGroup.alpha = 0;
        
        // 确保面板处于激活状态
        gameObject.SetActive(true);
        
        // 执行淡入动画
        DOTween.Sequence()
            .Append(canvasGroup.DOFade(1, 0.3f))
            .SetUpdate(true);  // 如果需要动画在时间暂停时继续运行，保留 SetUpdate(true)
    }

    /// <summary>
    /// 关闭面板
    /// </summary>
    public virtual void ClosePanel()
    {
        if (hasRemoved) return;  // 如果面板已经被移除，避免重复执行

        hasRemoved = true;
        
        // 执行淡出动画并销毁面板
        DOTween.Sequence()
            .Append(canvasGroup.DOFade(0, 0.3f))
            .OnComplete(() => Destroy(gameObject))
            .SetUpdate(true);  // 保持 SetUpdate(true) 如果需要动画在暂停时继续执行
    }
}