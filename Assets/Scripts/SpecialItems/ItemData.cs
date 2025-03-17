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
    public float cooldownTime; // 冷却时间
    
    public string requiredLevelName; // 该道具解锁所需的关卡名称
    public bool isUnlocked; // 是否解锁
    
    public ItemEffectType effectType; // 道具技能效果
    
    // 判断是否解锁
    public bool CheckUnlock(List<LevelData> levels)
    {
        LevelData requiredLevel = levels.Find(l => l.name == requiredLevelName);

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