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
    public string cardID; // 成就卡片的唯一标识符
    public string cardName; // 成就卡片名称
    [TextArea(1, 5)] public string cardDes; // 成就卡片描述，支持多行输入
    public Sprite cardSprite; // 成就卡片图片
    public bool isHeld; // 标记卡片是否已被持有
    public AchievementCondition condition; // 解锁该卡片的条件


    // 更新卡片的持有状态
    public void UpdateCardStatus(bool heldStatus)
    {
        isHeld = heldStatus;
    }

    public bool CheckCondition(string itemId)
    {
        if (itemId == condition.requiredItem)
        {
            return true;
        }
        else
        {
            return false;            
        }
    }

    // 更新UI
    public void ApplyVisuals(GameObject cardSlot)
    {
        cardSlot.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = cardName;

        cardSlot.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = cardDes;

        cardSlot.transform.GetChild(0).GetComponent<Image>().sprite = cardSprite;

        cardSlot.transform.GetChild(3).gameObject.SetActive(!isHeld);
    }
}


// 定义卡牌解锁条件的类
[System.Serializable]
public class AchievementCondition
{
    public string requiredItem; // 达成目标所需的道具 ID
}