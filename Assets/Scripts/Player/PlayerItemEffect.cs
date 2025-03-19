using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 使用道具的效果
/// </summary>
public class PlayerItemEffect : MonoBehaviour
{
    [SerializeField] private Transform intervalDecreaseFx;
    
    private StepManager stepManager;

    private float originInterval;

    private void Start()
    {
        intervalDecreaseFx.gameObject.SetActive(false);
        
        stepManager = FindObjectOfType<StepManager>();
    }

    public void UseEnergyMedicine()
    {
        if (stepManager == null) return;

        originInterval = stepManager.stepIncreaseInterval;
        
        stepManager.SetStepIncreaseInterval(0.5f);
        intervalDecreaseFx.gameObject.SetActive(true);
        
        StartCoroutine(ResetStepIncreaseIntervalAfterDelay());
    }

    private IEnumerator ResetStepIncreaseIntervalAfterDelay()
    {
        yield return new WaitForSeconds(10);
        
        intervalDecreaseFx.gameObject.SetActive(false);
        stepManager.SetStepIncreaseInterval(originInterval);
    }
}
