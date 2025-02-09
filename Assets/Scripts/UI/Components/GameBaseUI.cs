using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBaseUI : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Text stepText;

    [Header("配置颜色和样式")]
    [SerializeField] private Color zeroStepColor = Color.red;
    [SerializeField] private FontStyle zeroStepFontStyle = FontStyle.Bold;

    private Color defaultColor; // 默认颜色
    private FontStyle defaultFontStyle; // 默认字体样式

    private Coroutine colorCoroutine; // 用于平滑过渡颜色
    private Coroutine fontStyleCoroutine; // 用于平滑过渡字体样式

    private void Awake()
    {
        // 获取当前字体颜色和样式
        defaultColor = stepText.color;
        defaultFontStyle = stepText.fontStyle;

        pauseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPanel("SettingPanel");
        });
    }

    private void Start()
    {
        EVENTMGR.ChangeSteps += UpdateStepText;
    }

    private void OnDestroy()
    {
        EVENTMGR.ChangeSteps -= UpdateStepText;
    }

    private void UpdateStepText(int remainSteps)
    {
        stepText.text = "当前步数：" + remainSteps.ToString();

        // 颜色平滑过渡
        if (remainSteps == 0)
        {
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SmoothColorTransition(stepText.color, zeroStepColor, 0.5f));
            
            if (fontStyleCoroutine != null)
                StopCoroutine(fontStyleCoroutine);
            fontStyleCoroutine = StartCoroutine(SmoothFontStyleTransition(FontStyle.Bold, 0.5f));
        }
        else
        {
            if (colorCoroutine != null)
                StopCoroutine(colorCoroutine);
            colorCoroutine = StartCoroutine(SmoothColorTransition(stepText.color, defaultColor, 0.5f));
            
            if (fontStyleCoroutine != null)
                StopCoroutine(fontStyleCoroutine);
            fontStyleCoroutine = StartCoroutine(SmoothFontStyleTransition(FontStyle.Normal, 0.5f));
        }
    }

    // 平滑颜色过渡
    private IEnumerator SmoothColorTransition(Color startColor, Color endColor, float duration)
    {
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            stepText.color = Color.Lerp(startColor, endColor, timeElapsed / duration);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        stepText.color = endColor;
    }

    // 平滑字体样式过渡
    private IEnumerator SmoothFontStyleTransition(FontStyle targetStyle, float duration)
    {
        FontStyle currentStyle = stepText.fontStyle;
        FontStyle finalStyle = targetStyle;

        // 模拟从当前样式逐渐变到目标样式
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            // 字体加粗效果的模拟
            if (currentStyle == FontStyle.Normal && targetStyle == FontStyle.Bold)
            {
                float t = Mathf.Min(1f, timeElapsed / duration);
                stepText.fontStyle = t < 0.5f ? FontStyle.Normal : FontStyle.Bold;
            }
            else if (currentStyle == FontStyle.Bold && targetStyle == FontStyle.Normal)
            {
                float t = Mathf.Min(1f, timeElapsed / duration);
                stepText.fontStyle = t < 0.5f ? FontStyle.Bold : FontStyle.Normal;
            }

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        stepText.fontStyle = finalStyle;  // 确保最后一次为目标样式
    }
}
