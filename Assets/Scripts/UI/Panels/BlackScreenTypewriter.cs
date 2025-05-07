using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class BlackScreenTypewriter : BasePanel
{
    [Header("UI组件配置")] [SerializeField] private Image blackScreen;
    [SerializeField] private Text textDisplay;

    [Header("打字机设定")] [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float typeSpeed = 0.1f;
    [SerializeField] private float delayAfterComplete = 2f;

    private string fullText;
    private Action onCompleteCallback;

    private void Start()
    {
        StartTypewriter("原本只是要求“登记并保护历史遗迹”，\n却被盗墓团队误读为“收缴未申报文物以完成指标”... ... ");
    }

    public void StartTypewriter(string message)
    {
        fullText = message;
        StartCoroutine(TypewriterRoutine());
    }

    private IEnumerator TypewriterRoutine()
    {
        blackScreen.DOFade(1f, fadeDuration);
        yield return new WaitForSeconds(fadeDuration);
        
        textDisplay.text = "";
        
        for (int i = 0; i <= fullText.Length; i++)
        {
            textDisplay.text = fullText.Substring(0, i);
            yield return new WaitForSeconds(typeSpeed);
        }
        
        yield return new WaitForSeconds(delayAfterComplete);
        
        blackScreen.DOFade(0f, fadeDuration);
        textDisplay.text = "";
        
        SceneLoader.Instance.LoadScene(SceneName.LevelSelection,"...");
    }
}