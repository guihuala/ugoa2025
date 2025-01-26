using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EVENTMGR
{
    // 游戏需要用到的主要事件

    #region 点击物体或玩家

    // 点击角色事件
    public static event Action<bool> OnClickCharacter;

    // 时间流速管理事件
    public static event Action<float> OnTimeScaleChange;

    // 触发点击角色事件
    public static void TriggerClickCharacter(bool isActivity)
    {
        OnClickCharacter?.Invoke(isActivity);
    }

    // 触发时间流速改变事件
    public static void TriggerTimeScaleChange(float newTimeScale)
    {
        OnTimeScaleChange?.Invoke(newTimeScale);
    }

    #endregion

    #region 捡起物品

    public static event Action<string> OnCollectItem;

    public static void TriggerCollectItem(string itemId)
    {
        OnCollectItem?.Invoke(itemId);
    }    

    #endregion


}
