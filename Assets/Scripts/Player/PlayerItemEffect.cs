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
    [SerializeField]
    [Tooltip("时间减缓比例")]private float timeScaleSlow = 0.2f;
    
    private SlingshotManager slingshotManager;
    
    private StepManager stepManager;

    private float originInterval;
    
    private bool isSlingshotActive;
    
    private Player player;

    private void Start()
    {
        player = GetComponent<Player>();
        
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

        originInterval = stepManager.baseInterval;
        
        stepManager.SetStepIncreaseInterval(originInterval * 0.5f);
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

        // 发射弹弓的动画，默认动画会隐藏弹弓
        if (player.skin == Player.PlayerSkin.lv1)
        {
            player.PlayOverlayAnimation(4, "tool/with_catapult");
            player.PlayOverlayAnimation(2,"expressions/eyes/eyes_shut");
            player.PlayOverlayAnimation(3, "expressions/mouth/smile-v");
            player.PlayOverlayAnimation(5,"expressions/hat/hat_down-lv1");
        }else if (player.skin == Player.PlayerSkin.lv2)
        {
            player.PlayOverlayAnimation(4, "tool/with_catapult");
            player.PlayOverlayAnimation(2,"expressions/eyes/eyes-X");
            player.PlayOverlayAnimation(3, "expressions/mouth/smile-v");
        }
        else
        {
            player.PlayOverlayAnimation(3, "expressions/mouth/smile-v");
            player.PlayOverlayAnimation(4, "tool/hide_catpult", false, 0.1f, true);
        }

        EVENTMGR.TriggerTimeScaleChange(timeScaleSlow);
    }

    private void StopUsingSlingshot()
    {
        FindObjectOfType<PerspectiveCameraController>().allowCameraControl = true;
        FindObjectOfType<ClickableEffect>().Activate();
        
        slingshotManager.SetIsUsingSlingshot(false);
        isSlingshotActive = false;
        
        player.ClearTrack();
        
        EVENTMGR.TriggerTimeScaleChange(1.0f);
    }

    #endregion
}
