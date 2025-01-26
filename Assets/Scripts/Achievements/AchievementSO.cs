using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 脚本描述了成就系统需要使用到的各种结构
/// </summary>
[System.Serializable]
public class AchievementSO
{
    public string cardID;                      // 成就卡片的唯一标识符
    public string cardName;                    // 成就卡片名称
    [TextArea(1, 5)] public string cardDes;    // 成就卡片描述，支持多行输入
    public Sprite cardSprite;                  // 成就卡片图片
    public bool isHeld;                        // 标记卡片是否已被持有
    public AchievementCondition condition;            // 解锁该卡片的条件

    // 检查条件是否满足
    public bool CheckCondition(PlayerProgress progress)
    {
        // 调用条件的 Evaluate 方法，传入玩家进度
        return condition.Evaluate(progress);
    }

    // 更新卡片的持有状态
    public void UpdateCardStatus(bool heldStatus)
    {
        isHeld = heldStatus;
    }

    // 更新UI
    public void ApplyVisuals(GameObject cardSlot, Color heldColor, Color notHeldColor)
    {
        cardSlot.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = cardName;
        
        cardSlot.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = cardDes;
        
        cardSlot.transform.GetChild(0).GetComponent<Image>().sprite = cardSprite;
        
        cardSlot.transform.GetChild(4).gameObject.SetActive(!isHeld);
        
        cardSlot.transform.GetChild(0).GetComponent<Image>().color = isHeld ? heldColor : notHeldColor;
    }
}



// 定义卡牌解锁条件的类
[System.Serializable]
public class AchievementCondition
{
    public ConditionType conditionType; // 条件类型（进入次数或拾取道具）
    public string requiredItem;         // 达成目标所需的道具 ID

    // 评估当前条件是否满足
    public bool Evaluate(PlayerProgress progress)
    {
        // 根据条件类型执行相应逻辑
        switch (conditionType)
        {
            case ConditionType.ItemCollected:
                // 判断玩家是否已经获得所需的道具
                return progress.HasItem(requiredItem);

            default:
                return false;
        }
    }
}


// 定义条件类型的枚举
public enum ConditionType
{
    ItemCollected   // 收集特定道具
}

// 定义玩家进度管理的类
public class PlayerProgress
{
    public HashSet<string> inventory;  // 玩家已收集的道具，使用 HashSet 避免重复

    // 检查玩家是否拥有某个特定的道具
    public bool HasItem(string item)
    {
        return inventory.Contains(item);
    }

    // 添加道具到玩家的道具清单中
    public void AddItem(string item)
    {
        inventory.Add(item);
        Debug.Log("收集道具： " + item);
    }
}
