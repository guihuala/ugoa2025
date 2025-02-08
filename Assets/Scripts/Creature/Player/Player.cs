using System;
using System.Collections;
using System.Collections.Generic;
using Spine;
using UnityEngine;
using Spine.Unity;

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
    
    private bool isDead = false;
    public bool IsDead => isDead;
    
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
    public string eyeslv3Animation = "expressions/default_lv3";

    [Header("Spine 眼睛动画")]
    public string eyesXAnimation = "expressions/eyes/eyes-X";
    public string eyesShutAnimation = "expressions/eyes/eyes-shut";
    public string eyesBlinkAnimation = "expressions/eyes/eyes_blink";
    
    [Header("Spine 嘴动画")]
    public string defaultMouthAnimation = "expressions/mouth/mouth-default";
    public string trembleMouthAnimation = "expressions/mouth/mouth-tremble";
    public string VmouthAnimation = "expressions/mouth/smile-v";
    
    [Header("Spine 套动画")]
    private string sinkAnimation = "sets/struggle";
    private string saluteAnimation = "sets/salute";
    
    [Header("Spine 动画轨道配置")]
    private int baseTrack = 1;    // 主动画轨道
    private int eyesTrack = 2;    // 眼睛
    private int mouseTrack = 3;   // 嘴
    
    // 用于射线检测特殊物体
    private Dictionary<int, Collider> currentSpecialItems = new Dictionary<int, Collider>();


    private void Awake()
    {
        skeletonAnimation = GetComponentInChildren<SkeletonAnimation>();
    }

    private void Start()
    {
        EVENTMGR.OnStepIntoGrass += SetInvisible;
        EVENTMGR.OnEnterSwamp += HandleSwampEnter;
        EVENTMGR.OnExitSwamp += HandleSwampExit;
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
    /// 播放眼睛动画；不输入参数则播放默认眼睛动画
    /// </summary>
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
                        selectedEyeAnimation = $"{eyesBlinkAnimation}";
                        break;
                }                
            }
            
            PlayOverlayAnimation(eyesTrack, selectedEyeAnimation, true, 0.1f);
        }
    }

    public void PlayOverlayAnimation(int trackIndex, string animName, bool loop = false, float mixDuration = 0.1f)
    {
        if (skeletonAnimation != null && skeletonAnimation.state != null)
        {
            skeletonAnimation.state.Data.DefaultMix = mixDuration;
            skeletonAnimation.state.SetAnimation(trackIndex, animName, loop);
        }
    }

    public void ClearTrack()
    {
        skeletonAnimation.state.ClearTrack(baseTrack);
        skeletonAnimation.state.ClearTrack(eyesTrack);
        skeletonAnimation.state.ClearTrack(mouseTrack);
        
        skeletonAnimation.Skeleton.SetToSetupPose();
        
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
        
        if(skin == PlayerSkin.lv3)
            return;
        
        PlayEyesAnimation(eyesXAnimation);
    }

    private void HandleSwampExit()
    {
        isInSwamp = false;
        stayTime = 0f;
        ResetHeightPosition();
        
        ClearTrack();
    }
    

    private void HandleSwampStay()
    {
        if (!isInSwamp || isDead) return;
    
        stayTime += Time.deltaTime;
        transform.position += Vector3.down * sinkSpeed * Time.deltaTime;

        if (stayTime >= timeUntilDeath)
        {
            isDead = true;
            EVENTMGR.TriggerPlayerDead();
        }

        EVENTMGR.TriggerChangeSwampProgress(1 - stayTime / timeUntilDeath);
    }

    
    private void ResetHeightPosition()
    {
        transform.position = new Vector3(transform.position.x, initialHeight, transform.position.z);
    }

    #endregion

    #region 射线检测
    
    private void Update()
    {
        HandleSwampStay();
        
        float detectionDistance = 2f;
        Dictionary<int, Collider> detectedItems = new Dictionary<int, Collider>();

        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.collider.GetComponent<IEnterSpecialItem>() != null ||
                hit.collider.GetComponent<IExitSpecialItem>() != null)
            {
                int id = hit.collider.GetInstanceID();
                if (!detectedItems.ContainsKey(id))
                {
                    detectedItems.Add(id, hit.collider);
                }
            }
            else
            {
                if(isInSwamp)
                    EVENTMGR.TriggerExitSwamp();
            }
        }
        
        foreach (var kvp in detectedItems)
        {
            if (!currentSpecialItems.ContainsKey(kvp.Key))
            {
                IEnterSpecialItem enterItem = kvp.Value.GetComponent<IEnterSpecialItem>();
                if (enterItem != null)
                {
                    enterItem.Apply();
                }
            }
        }
        
        foreach (var kvp in currentSpecialItems)
        {
            if (!detectedItems.ContainsKey(kvp.Key))
            {
                IExitSpecialItem exitItem = kvp.Value.GetComponent<IExitSpecialItem>();
                if (exitItem != null)
                {
                    exitItem.UnApply();
                }
            }
        }
        
        currentSpecialItems = detectedItems;
    }

    #endregion
    
    private void PlayerDead()
    {
        if (isInvisible)
            return;
        
        isInSwamp = false;
        StartCoroutine(DelayedPlayerDeath());
    }
    
    private IEnumerator DelayedPlayerDeath()
    {
        ClearTrack();
        PlayOverlayAnimation(eyesTrack, eyesXAnimation);
        yield return new WaitForSeconds(deathDelay);
        UIManager.Instance.OpenPanel("GameFailurePanel");
    }
     
    private void OnDestroy()
    {
        EVENTMGR.OnStepIntoGrass -= SetInvisible;
        EVENTMGR.OnEnterSwamp -= HandleSwampEnter;
        EVENTMGR.OnExitSwamp -= HandleSwampExit;
        EVENTMGR.OnPlayerDead -= PlayerDead;
    }

}
