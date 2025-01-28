using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    private bool isInvisible;
    public bool IsInvisible => isInvisible;

    [Header("沼泽下沉相关配置")]
    [SerializeField] private float sinkSpeed = 0.01f; // 下沉速度
    [SerializeField] private float timeUntilDeath = 5f; // 停留多久会死亡
    private float stayTime = 0f; // 玩家在沼泽中累计停留时间
    private bool isInSwamp = false;
    private float initialHeight;
    

    private void Start()
    {
        EVENTMGR.OnStepIntoGrass += SetInvisible;
        EVENTMGR.OnEnterSwamp += HandleSwampEnter;
        EVENTMGR.OnExitSwamp += HandleSwampExit;
        EVENTMGR.OnStayInSwamp += HandleSwampStay;

        initialHeight = transform.position.y;
    }

    #region 隐身

    public void SetInvisible(bool value)
    {
        if (isInvisible != value)
        {
            isInvisible = value;
            OnStealthStateChanged(isInvisible);
        }
    }

    private void EnableStealth()
    {
        SetInvisible(true);
    }

    private void DisableStealth()
    {
        SetInvisible(false);
    }

    // 隐身的视觉特效
    private void OnStealthStateChanged(bool isInvisible)
    {
        SpriteRenderer renderer = GetComponentInChildren<SpriteRenderer>();
        if (renderer != null)
        {
            Color color = renderer.color;
            // 可以改成shader
            color = isInvisible ? Color.gray : Color.white;
            renderer.color = color;
        }
    }

    #endregion

    #region 沼泽下沉和死亡逻辑

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
            DieInSwamp();
        }
        
        EVENTMGR.TriggerChangeSwampProgress(1- stayTime / timeUntilDeath);
    }

    private void DieInSwamp()
    {
        isInSwamp = false;
        
        UIManager.Instance.OpenPanel("GameFailurePanel");
    }
    
    private void ResetHeightPosition()
    {
        // 重置玩家的高度位置
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
    
    private void OnDestroy()
    {
        EVENTMGR.OnStepIntoGrass -= SetInvisible;
        EVENTMGR.OnEnterSwamp -= HandleSwampEnter;
        EVENTMGR.OnExitSwamp -= HandleSwampExit;
        EVENTMGR.OnStayInSwamp -= HandleSwampStay;
    }
}
