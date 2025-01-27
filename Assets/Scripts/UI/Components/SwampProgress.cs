using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SwampProgress : MonoBehaviour
{
    [Header("UI组件")] [SerializeField] private Image BG;
    [SerializeField] private Image Fill;

    private void Start()
    {
       Reset();

        EVENTMGR.OnChangeSwampProgress += ChangeSwampProgress;
        EVENTMGR.OnExitSwamp += Reset;
    }

    private void Reset()
    {
        if (Fill != null)
        {
            Fill.fillAmount = 1;
        }
    }

    private void ChangeSwampProgress(float percent)
    {
        if (Fill != null)
        {
            Fill.fillAmount = percent;
        }
    }

}