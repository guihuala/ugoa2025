using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SkyboxManager : MonoBehaviour
{
    [Header("基本配置")]
    public float transitionTime = 0.5f;   // 切换位置的过渡时间

    [Header("材质配置")]
    public Material skyboxMaterial;
    public Color[] targetColor0;
    public Color[] targetColor1;
    
    private Color originColor0;
    private Color originColor1;   
    
    private void Start()
    {
        // 初始设置天空盒颜色
        StartChangeColor(true);
        
        EVENTMGR.OnElevatorMove += StartChangeColor;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnElevatorMove -= StartChangeColor;
    }

    private void StartChangeColor(bool isMoveUp)
    {
        if (isMoveUp)
        {
            StartCoroutine(SmoothMove(0,transitionTime));
        }
        else
        {
            StartCoroutine(SmoothMove(1,transitionTime));
        }
    }

    private IEnumerator SmoothMove(int index, float duration)
    {
        float elapsedTime = 0f;

        // 在过渡期间平滑变换颜色
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;

            // 平滑插值过渡颜色
            Color newColor0 = Color.Lerp(originColor0, targetColor0[index], t);
            Color newColor1 = Color.Lerp(originColor1, targetColor1[index], t);

            // 设置天空盒的颜色
            skyboxMaterial.SetColor("_Color0", newColor0);
            skyboxMaterial.SetColor("_Color1", newColor1);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 确保天空盒的颜色完全达到目标颜色
        skyboxMaterial.SetColor("_Color0", targetColor0[index]);
        skyboxMaterial.SetColor("_Color1", targetColor1[index]);

        originColor0 = targetColor0[index];
        originColor1 = targetColor1[index];
    }
}
