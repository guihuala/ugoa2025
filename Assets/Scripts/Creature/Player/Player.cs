using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using UnityEngine.Serialization;

public class Player : MonoBehaviour
{
    public enum PlayerSkin
    {
        lv1,
        lv2,
        lv3
    }
    
    private bool isInvisible;
    public bool IsInvisible => isInvisible;
    
    
    private bool isInSwamp = false;
    public bool IsInSwamp => isInSwamp;
    
    private float stayTime = 0f; // 玩家在沼泽中累计停留时间 
    
    [Header("沼泽下沉相关配置")]
    [SerializeField] private float sinkSpeed = 0.01f; // 下沉速度
    [SerializeField] private float timeUntilDeath = 5f; // 停留多久会死亡
    
    [Header("死亡设置")]
    [SerializeField] private float deathDelay = 1.0f;

    private float initialHeight = 1.5f;
    private SkeletonAnimation skeletonAnimation;
    private string currentAnimation = "";

    [Header("Player 皮肤")]
    public PlayerSkin skin = PlayerSkin.lv1;
    
    [Header("Spine 移动动画")]
    public string walkAnimation = "move/walk";
    public string standAnimation = "move/standby";
    public string sneakAnimation = "move/sneak";
    public string hatAnimation = "move/hat";
    
    [Header("Spine 默认眼睛")]
    public string eyeslv1Animation = "expressions/default-lv1";
    public string eyeslv2Animation = "expressions/default-lv2";
    public string eyeslv3Animation = "expressions/default-lv3";

    [Header("Spine 眼睛动画")]
    public string eyesXAnimation = "expressions/eyes/eyes-X";
    public string eyesShutAnimation = "expressions/eyes/eyes-shut";
    
    [Header("Spine 嘴动画")]
    public string defaultMouthAnimation = "expressions/mouth/mouth-default";
    public string trembleMouthAnimation = "expressions/mouth/mouth-tremble";
    public string VmouthAnimation = "expressions/mouth/smile-v";
    
    [Header("Spine 套动画")]
    private string sinkAnimation = "sets/struggle";
    private string saluteAnimation = "sets/salute";
    
    
    [Header("Spine 动画轨道配置")]
    private int baseTrack = 0;    // 动作
    private int eyesTrack = 1; // 眼睛
    private int mouseTrack = 2; // 嘴
    

    private void Start()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();

        EVENTMGR.OnStepIntoGrass += SetInvisible;
        EVENTMGR.OnEnterSwamp += HandleSwampEnter;
        EVENTMGR.OnExitSwamp += HandleSwampExit;
        EVENTMGR.OnStayInSwamp += HandleSwampStay;
        EVENTMGR.OnPlayerDead += PlayerDead;
        
        PlayAnimation(standAnimation);
        PlayEyesAnimation();
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

    /// <summary>
    /// 播放眼睛动画 不输入参数则播放默认眼睛动画
    /// </summary>
    /// <param name="animName">动画名</param>
    public void PlayEyesAnimation(string animName = "default")
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            string selectedEyeAnimation = animName;

            if (selectedEyeAnimation == "default")
            {
                switch (skin)
                {
                    case PlayerSkin.lv1:
                        selectedEyeAnimation = $"{eyeslv1Animation}";
                        break;
                    case PlayerSkin.lv2:
                        selectedEyeAnimation = $"{eyeslv2Animation}";
                        break;
                    case PlayerSkin.lv3:
                        selectedEyeAnimation = $"{eyeslv3Animation}";
                        break;
                }                
            }
            
            PlayOverlayAnimation(eyesTrack, selectedEyeAnimation, true, 0.1f);
        }
    }


    // 叠加动画
    public void PlayOverlayAnimation(int trackIndex, string animName, bool loop = false, float mixDuration = 0.1f)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            // 设置轨道混合时间
            skeletonAnimation.state.Data.DefaultMix = mixDuration;
            skeletonAnimation.state.SetAnimation(trackIndex, animName, loop);
        }
    }

    public void ClearTrack()
    {
        skeletonAnimation.state.ClearTrack(baseTrack);
        skeletonAnimation.state.ClearTrack(eyesTrack);
        skeletonAnimation.state.ClearTrack(mouseTrack);
        
        PlayAnimation(standAnimation, true);
        PlayEyesAnimation();
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
        PlayEyesAnimation(eyesXAnimation);
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
        // 如果处于隐身状态，则不触发死亡
        if (isInvisible)
            return;
        
        isInSwamp = false;
        // 延迟触发死亡事件
        StartCoroutine(DelayedPlayerDeath());
    }
    
    private IEnumerator DelayedPlayerDeath()
    {
        yield return new WaitForSeconds(deathDelay);
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
