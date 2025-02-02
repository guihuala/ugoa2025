using System;
using UnityEngine;
using Spine.Unity;

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

    [Header("Spine 动画名称")]
    public string walkAnimation = "walk";

    private void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        EVENTMGR.OnStepIntoGrass += SetInvisible;
        EVENTMGR.OnEnterSwamp += HandleSwampEnter;
        EVENTMGR.OnExitSwamp += HandleSwampExit;
        EVENTMGR.OnStayInSwamp += HandleSwampStay;
        EVENTMGR.OnPlayerDead += PlayerDead;
        
        ClearAnimation();
    }

    #region Spine动画控制

    public void PlayAnimation(string animName, bool loop)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            if (currentAnimation == animName) return; // 避免重复播放相同动画
            skeletonAnimation.state.SetAnimation(0, animName, loop);
            currentAnimation = animName;
        }
    }

    public void ClearAnimation()
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            skeletonAnimation.state.ClearTrack(0);
            currentAnimation = "";
        }
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
    }

    #endregion

    #region 沼泽下沉

    private void HandleSwampEnter()
    {
        isInSwamp = true;
    }
    
    private void HandleSwampExit()
    {
        isInSwamp = false;
        stayTime = 0f; // 重置停留时间
        ResetHeightPosition();
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
