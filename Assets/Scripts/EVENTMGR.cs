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

    /// <summary>
    /// 角色经过图块，用于显示脚印
    /// </summary>
    public static Action<Vector3> OnPlayerStep;

    public static void TriggerClickMarker(Vector3 pos)
    {
        OnClickMarker?.Invoke(pos);
    }

    /// <summary>
    /// 触发角色经过图块事件
    /// </summary>
    /// <param name="pos">图块坐标</param>
    public static void TriggerPlayerStep(Vector3 pos)
    {
        OnPlayerStep?.Invoke(pos);
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

    public static event Action<float> OnChangeSwampProgress;
    
    public static void TriggerEnterSwamp()
    {
        OnEnterSwamp?.Invoke();
    }
    
    public static void TriggerExitSwamp()
    {
        OnExitSwamp?.Invoke();
    }
    public static void TriggerChangeSwampProgress(float percent)
    {
        OnChangeSwampProgress?.Invoke(percent);
    }

    #endregion

    #region 进入触发目标地块

    public static Action<Vector3> OnEnterTargetField;

    public static void TriggerEnterTargetField(Vector3 pos)
    {
        OnEnterTargetField?.Invoke(pos);
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

    #region 玩家失败

    public static event Action OnPlayerDead;
    
    public static void TriggerPlayerDead(string reason)
    {
        LevelManager.Instance.failureReason = reason;
        OnPlayerDead?.Invoke();
    }
    
    /// <summary>
    /// 屏幕震动效果
    /// </summary>
    public static event Action OnPlayerFound;
    
    public static void TriggerPlayerFound()
    {
        OnPlayerFound?.Invoke();
    }

    #endregion

    #region 解锁道具

    public static event Action<ItemData> OnUnlockItem;

    public static void TriggerUnlockItem(ItemData item)
    {
        OnUnlockItem?.Invoke(item);
    }

    #endregion
    
    #region 使用弹弓

    public static event Action OnUsingSlingshot;
    
    public static void TriggerUsingSlingshot()
    {
        OnUsingSlingshot?.Invoke();
    }

    #endregion

    #region 电梯上升/下降

    public static event Action<bool> OnElevatorMove;
    
    public static void TriggerElevatorMove(bool isMoveUp)
    {
        OnElevatorMove?.Invoke(isMoveUp);
    }

    #endregion

    #region 地形改变

    public static event Action OnTerrainChange;
    
    public static void TriggerTerrainChange()
    {
        OnTerrainChange?.Invoke();
    }

    #endregion
}
