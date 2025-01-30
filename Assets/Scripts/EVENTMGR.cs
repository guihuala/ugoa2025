using System;
using UnityEngine;

public static class EVENTMGR
{
    // 游戏需要用到的主要事件

    #region 点击物体或玩家

    // 点击角色事件
    public static event Action<bool> OnClickPlayer;

    // 时间流速管理事件
    public static event Action<float> OnTimeScaleChange;
    
    public static event Action OnClickPath;

    // 触发点击角色事件
    public static void TriggerClickPlayer(bool isActivity)
    {
        OnClickPlayer?.Invoke(isActivity);
    }

    // 触发时间流速改变事件
    public static void TriggerTimeScaleChange(float newTimeScale)
    {
        OnTimeScaleChange?.Invoke(newTimeScale);
    }

    public static void TriggerClickPath()
    {
        OnClickPath?.Invoke();
    }

    #endregion

    #region 点击地图

    public static Action<Vector3> OnClickMarker;

    public static void TriggerClickMarker(Vector3 pos)
    {
        OnClickMarker?.Invoke(pos);
    }

    #endregion

    #region 捡起物品

    public static event Action<string> OnCollectItem;

    public static void TriggerCollectItem(string itemId)
    {
        OnCollectItem?.Invoke(itemId);
    }    

    #endregion

    #region 隐身效果

    public static event Action<bool> OnStepIntoGrass;    
    
    public static void TriggerStepIntoGrass(bool isVisible)
    {
        OnStepIntoGrass?.Invoke(isVisible);
    }

    #endregion

    #region 进入沼泽

    public static event Action OnEnterSwamp;
    
    public static event Action OnExitSwamp;
    
    public static event Action<float> OnStayInSwamp;

    public static event Action<float> OnChangeSwampProgress;
    
    public static void TriggerEnterSwamp()
    {
        OnEnterSwamp?.Invoke();
    }
    
    public static void TriggerExitSwamp()
    {
        OnExitSwamp?.Invoke();
    }
    
    public static void TriggerStayInSwamp(float deltaTime)
    {
        OnStayInSwamp?.Invoke(deltaTime);
    }
    
    public static void TriggerChangeSwampProgress(float percent)
    {
        OnChangeSwampProgress?.Invoke(percent);
    }

    #endregion
    
    #region 步数

    public static event Action<int> OnUseStep;
    public static event Action<int> ChangeSteps;

    public static void TriggerUseStep(int step)
    {
        OnUseStep?.Invoke(step);
    }
    
    public static void TriggerChangeSteps(int remainSteps)
    {
        ChangeSteps?.Invoke(remainSteps);
    }   

    #endregion

    #region 玩家失败或胜利

    public static event Action OnPlayerDead;
    
    public static event Action OnPLayerVictory;

    public static void TriggerPlayerDead()
    {
        OnPlayerDead?.Invoke();
    }

    public static void TriggerPLayerVictory()
    {
        OnPLayerVictory?.Invoke();
    }

    #endregion
}
