using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用道具的效果
/// </summary>
public class PlayerItemEffect : MonoBehaviour
{
    private StepManager stepManager;

    private float originInterval;

    private void Start()
    {
        stepManager = FindObjectOfType<StepManager>();
    }

    public void UseEnergyMedicine()
    {
        if (stepManager == null) return;

        originInterval = stepManager.stepIncreaseInterval;
        
        stepManager.SetStepIncreaseInterval(0.5f);
        StartCoroutine(ResetStepIncreaseIntervalAfterDelay());
    }

    private IEnumerator ResetStepIncreaseIntervalAfterDelay()
    {
        yield return new WaitForSeconds(10);
        stepManager.SetStepIncreaseInterval(originInterval);
    }
}
