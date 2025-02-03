using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using DG.Tweening;

public class SwampProgress : MonoBehaviour
{
    [Header("UI组件")]
    [SerializeField] private Image BG;
    [SerializeField] private Image Fill;

    [Header("渐变色配置")]
    [SerializeField] private Gradient colorGradient;
    [SerializeField] private float animationDuration = 0.5f;

    private void Start()
    {
        Reset();

        EVENTMGR.OnChangeSwampProgress += ChangeSwampProgress;
        EVENTMGR.OnExitSwamp += Reset;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnChangeSwampProgress -= ChangeSwampProgress;
        EVENTMGR.OnExitSwamp -= Reset;
    }

    private void Reset()
    {
        ChangeSwampProgress(1);
    }

    private void ChangeSwampProgress(float percent)
    {
        if (Fill != null)
        {
            Fill.DOFillAmount(percent, animationDuration);
            Color targetColor = colorGradient.Evaluate(percent);
            // 平滑过渡到目标颜色
            Fill.DOColor(targetColor, animationDuration);
        }
    }
}