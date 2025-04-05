using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI引导遮罩组件
/// 功能：在UI上创建半透明遮罩，只留出目标区域可见，并实现点击穿透效果
/// </summary>
public class GuideMask : MaskableGraphic, ICanvasRaycastFilter
{
    public static GuideMask Self; // 单例实例
    
    private RectTransform _target;      // 当前高亮的目标UI
    private Vector2 _targetMin;         // 目标区域最小坐标(左下)
    private Vector2 _targetMax;         // 目标区域最大坐标(右上)
    private RectTransform _targetArea;  // 用于复制目标区域参数的RectTransform

    public event Action OnClickOutside;

    /// <summary>
    /// 射线检测过滤(实现ICanvasRaycastFilter接口)
    /// 只有点击在遮罩区域时才响应，目标区域穿透
    /// </summary>
    public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
    {
        // 当点击位置不在目标区域内时返回true(允许事件穿透)
        return !RectTransformUtility.RectangleContainsScreenPoint(_targetArea, sp, eventCamera);
    }

    public void OnClickButton()
    {
        OnClickOutside?.Invoke();
    }

    /// <summary>
    /// 关闭引导遮罩
    /// </summary>
    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 显示引导遮罩并高亮指定UI
    /// </summary>
    /// <param name="target">要高亮的目标UI</param>
    public void Play(RectTransform target)
    {
        gameObject.SetActive(true);

        // 将目标的世界坐标转换为屏幕坐标
        var screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, target.position);

        // 将屏幕坐标转换为本地坐标
        Vector2 localPoint;
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, screenPoint, Camera.main,
                out localPoint))
        {
            Close();
            return;
        }

        // 复制目标UI的所有RectTransform参数到_targetArea
        _targetArea.anchorMax = target.anchorMax;
        _targetArea.anchorMin = target.anchorMin;
        _targetArea.anchoredPosition = target.anchoredPosition;
        _targetArea.anchoredPosition3D = target.anchoredPosition3D;
        _targetArea.offsetMax = target.offsetMax;
        _targetArea.offsetMin = target.offsetMin;
        _targetArea.pivot = target.pivot;
        _targetArea.sizeDelta = target.sizeDelta;
        _targetArea.localPosition = localPoint;

        // 强制立即更新RectTransform
        _targetArea.ForceUpdateRectTransforms();
        _target = _targetArea;
        _target.ForceUpdateRectTransforms();
        
        // 立即刷新视图
        LateUpdate();
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    public void Init()
    {
        // 查找目标区域子对象
        _targetArea = gameObject.transform.Find("TargetArea") as RectTransform;
        Self = this;  // 设置单例
        Close();      // 默认关闭
    }

    /// <summary>
    /// 构建遮罩网格(重写MaskableGraphic方法)
    /// 创建"回"字形网格，中间挖空目标区域
    /// </summary>
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        toFill.Clear();  // 清空现有网格

        var maskRect = rectTransform.rect;  // 获取遮罩矩形范围

        // 计算遮罩矩形的四个角点(本地坐标)
        var maskRectLeftTop = new Vector2(-maskRect.width / 2, maskRect.height / 2);
        var maskRectLeftBottom = new Vector2(-maskRect.width / 2, -maskRect.height / 2);
        var maskRectRightTop = new Vector2(maskRect.width / 2, maskRect.height / 2);
        var maskRectRightBottom = new Vector2(maskRect.width / 2, -maskRect.height / 2);

        // 计算目标区域的四个角点
        var targetRectLeftTop = new Vector2(_targetMin.x, _targetMax.y);
        var targetRectLeftBottom = _targetMin;
        var targetRectRightTop = _targetMax;
        var targetRectRightBottom = new Vector2(_targetMax.x, _targetMin.y);

        // 添加8个顶点(按顺时针方向)
        toFill.AddVert(maskRectLeftBottom, color, Vector2.zero);    // 0:遮罩左下
        toFill.AddVert(targetRectLeftBottom, color, Vector2.zero);  // 1:目标左下
        toFill.AddVert(targetRectRightBottom, color, Vector2.zero);  // 2:目标右下
        toFill.AddVert(maskRectRightBottom, color, Vector2.zero);   // 3:遮罩右下
        toFill.AddVert(targetRectRightTop, color, Vector2.zero);    // 4:目标右上
        toFill.AddVert(maskRectRightTop, color, Vector2.zero);     // 5:遮罩右上
        toFill.AddVert(targetRectLeftTop, color, Vector2.zero);     // 6:目标左上
        toFill.AddVert(maskRectLeftTop, color, Vector2.zero);      // 7:遮罩左上

        // 添加8个三角形(组成"回"字形)
        // 左下矩形
        toFill.AddTriangle(0, 1, 2);
        toFill.AddTriangle(2, 3, 0);
        // 右下矩形
        toFill.AddTriangle(3, 2, 4);
        toFill.AddTriangle(4, 5, 3);
        // 右上矩形
        toFill.AddTriangle(6, 7, 5);
        toFill.AddTriangle(5, 4, 6);
        // 左上矩形
        toFill.AddTriangle(7, 6, 1);
        toFill.AddTriangle(1, 0, 7);
    }

    /// <summary>
    /// 每帧最后更新视图
    /// </summary>
    void LateUpdate()
    {
        RefreshView();
    }

    /// <summary>
    /// 刷新遮罩视图
    /// </summary>
    private void RefreshView()
    {
        Vector2 newMin;
        Vector2 newMax;
        
        if (_target != null && _target.gameObject.activeSelf)
        {
            // 计算目标区域相对于遮罩的边界
            var bounds = RectTransformUtility.CalculateRelativeRectTransformBounds(transform, _target);
            newMin = bounds.min;
            newMax = bounds.max;
        }
        else
        {
            newMin = Vector2.zero;
            newMax = Vector2.zero;
        }

        // 如果边界发生变化，则重新绘制
        if (_targetMin != newMin || _targetMax != newMax)
        {
            _targetMin = newMin;
            _targetMax = newMax;
            SetAllDirty();  // 标记为需要重新绘制
        }
    }
}