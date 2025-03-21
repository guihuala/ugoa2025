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
    
    private SlingshotManager slingshotManager;
    
    private StepManager stepManager;

    private float originInterval;
    
    private bool isSlingshotActive;

    private void Start()
    {
        intervalDecreaseFx.gameObject.SetActive(false);
        
        slingshotManager = FindObjectOfType<SlingshotManager>();
        stepManager = FindObjectOfType<StepManager>();

        EVENTMGR.OnUsingSlingshot += UseSlingshot;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnUsingSlingshot -= UseSlingshot;
    }

    #region 能量药剂

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

    #endregion

    #region 弹弓

    public void UseSlingshot()
    {
        if (isSlingshotActive)
        {
            StopUsingSlingshot();
        }
        else
        {
            StartUsingSlingshot();
        }
    }
    
    private void StartUsingSlingshot()
    {
        FindObjectOfType<PerspectiveCameraController>().allowCameraControl = false;
        FindObjectOfType<ClickableEffect>().Deactivate();
        
        slingshotManager.SetIsUsingSlingshot(true);
        isSlingshotActive = true;
    }

    private void StopUsingSlingshot()
    {
        FindObjectOfType<PerspectiveCameraController>().allowCameraControl = true;
        FindObjectOfType<ClickableEffect>().Activate();
        
        slingshotManager.SetIsUsingSlingshot(false);
        isSlingshotActive = false;
    }

    #endregion
}
