using System;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    private bool isInvisible;
    public bool IsInvisible => isInvisible;

    [Header("沼泽下沉相关配置")]
    [SerializeField] private float sinkSpeed = 0.01f; // 下沉速度
    [SerializeField] private float timeUntilDeath = 5f; // 停留多久会死亡
    private float stayTime = 0f; // 玩家在沼泽中累计停留时间
    private bool isInSwamp = false;
    private float initialHeight = 1.5f;
    
    private SkeletonAnimation skeletonAnimation;
    private string currentAnimation = "";

    [Header("Spine 移动动画")]
    public string walkAnimation = "move/walk";
    public string standAnimation = "move/standby";
    public string sneakAnimation = "move/sneak";
    
    [Header("Spine 表情动画")]
    public string eyesShutAnimation = "expression/eyes_shut";
    public string mouseTrembleAnimation = "expression/mouse_tremble";
    public string mouseDefaultAnimation = "expression/mouse_default";
    
    [Header("Spine 套动画")]
    private string sinkAnimation = "sets/struggle";
    private string saluteAnimation = "sets/salute";
    
    
    [Header("Spine 动画轨道配置")]
    private int baseTrack = 0;    // 基础动画轨道（移动、待机）
    private int overlayTrack = 1; // 叠加动画轨道（表情、特殊动作）
    

    private void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        EVENTMGR.OnStepIntoGrass += SetInvisible;
        EVENTMGR.OnEnterSwamp += HandleSwampEnter;
        EVENTMGR.OnExitSwamp += HandleSwampExit;
        EVENTMGR.OnStayInSwamp += HandleSwampStay;
        EVENTMGR.OnPlayerDead += PlayerDead;
        
        ClearTrack();
    }

    #region Spine动画控制
    
    public void PlayAnimation(string animName, bool loop = true)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            var currentTrack = skeletonAnimation.state.GetCurrent(baseTrack);
            if (currentTrack != null && currentTrack.Animation.Name == animName) 
                return;

            skeletonAnimation.state.SetAnimation(baseTrack, animName, loop);
        }
    }

    // 叠加动画
    public void PlayOverlayAnimation(string animName, bool loop = false, float mixDuration = 0.1f)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            // 设置轨道混合时间
            skeletonAnimation.state.Data.DefaultMix = mixDuration;
            skeletonAnimation.state.SetAnimation(1, animName, loop);
        }
    }
    
    public void ClearTrack()
    {
        skeletonAnimation.state.ClearTrack(baseTrack);
        skeletonAnimation.state.ClearTrack(overlayTrack);
        
        PlayAnimation(standAnimation, true);
    }

    #endregion
    
    #region 隐身功能

    public void SetInvisible(bool value)
    {
        if (isInvisible != value)
        {
            isInvisible = value;
            OnStealthStateChanged(isInvisible);
        }
    }

    private void OnStealthStateChanged(bool isInvisible)
    {
        SpineTransparencyController spineTransparencyController = GetComponentInChildren<SpineTransparencyController>();
        if (spineTransparencyController != null)
        {
            spineTransparencyController.SetTransparency(isInvisible ? 0.5f : 1f);
        }
        
        if (isInvisible)
        {
            PlayAnimation(sneakAnimation);
        }
        else
        {
            ClearTrack();
        }
    }


    #endregion

    #region 沼泽下沉

    private void HandleSwampEnter()
    {
        isInSwamp = true;
        
        PlayAnimation(sinkAnimation, true);
    }

    private void HandleSwampExit()
    {
        isInSwamp = false;
        stayTime = 0f; // 重置停留时间
        ResetHeightPosition();
        
        ClearTrack();
    }


    private void HandleSwampStay(float duration)
    {
        if (!isInSwamp) return;
        
        stayTime += duration;
        
        // 根据停留时间改变玩家位置
        transform.position += Vector3.down * sinkSpeed * duration;

        if (stayTime >= timeUntilDeath)
        {
            EVENTMGR.TriggerPlayerDead();
        }
        
        EVENTMGR.TriggerChangeSwampProgress(1 - stayTime / timeUntilDeath);
    }
    
    private void ResetHeightPosition()
    {
        transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);
    }

    #endregion

    #region 触发器检测

    private void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<IEnterSpecialItem>();
        if (item != null)
        {
            item.Apply();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var item = other.GetComponent<IExitSpecialItem>();
        if (item != null)
        {
            item.UnApply();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        var stayItem = other.GetComponent<IStayInSpecialItem>();
        if (stayItem != null)
        {
            stayItem.Stay(Time.deltaTime);
        }
    }

    #endregion
    
    private void PlayerDead()
    {
        if (isInvisible)
            return;
        
        isInSwamp = false;
        UIManager.Instance.OpenPanel("GameFailurePanel");
    }  
     
    private void OnDestroy()
    {
        EVENTMGR.OnStepIntoGrass -= SetInvisible;
        EVENTMGR.OnEnterSwamp -= HandleSwampEnter;
        EVENTMGR.OnExitSwamp -= HandleSwampExit;
        EVENTMGR.OnStayInSwamp -= HandleSwampStay;
        EVENTMGR.OnPlayerDead -= PlayerDead;
    }
}
