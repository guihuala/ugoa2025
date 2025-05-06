using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemEffectType
{
    slingshot,
    energyMedicine,
}

[CreateAssetMenu(fileName = "NewItem", menuName = "Item System/Item")]
public class ItemData : ScriptableObject
{
    public string itemName; // 道具名称
    public Sprite icon; // 图标
    [TextArea] public string description; // 介绍
    
    public float cooldownTime; // 冷却时间
    public float effectDuration; // 生效时长

    public string requiredLevelName; // 该道具解锁所需的关卡名称
    public bool isUnlocked; // 是否解锁

    public ItemEffectType effectType; // 道具技能效果

    public void UnlockItem()
    {
        isUnlocked = true;
        EVENTMGR.TriggerUnlockItem(this);
    }
    
    // 判断是否解锁
    public bool CheckUnlock()
    {
        LevelData requiredLevel = LevelManager.Instance.levels.Find(l => l.name == requiredLevelName);

        if (requiredLevel != null && requiredLevel.isUnlocked && requiredLevel.isPlayed)
        {
            isUnlocked = true;
        }
        else
        {
            isUnlocked = false;
        }

        return isUnlocked;
    }
}