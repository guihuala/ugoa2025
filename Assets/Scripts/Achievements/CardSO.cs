using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum CardClass
{
    Human,
    Orc,
    Undead,
}

[System.Serializable]
public class CardSO
{
    public string cardID;
    public string cardName;
    [TextArea(1, 5)] public string cardDes;
    public Sprite cardSprite;
    public CardClass cardClass;
    public bool isHeld;
    public CardCondition condition;

    public bool CheckCondition(PlayerProgress progress)
    {
        return condition.Evaluate(progress);
    }

    public void UpdateCardStatus(bool heldStatus)
    {
        isHeld = heldStatus;
    }

    public void ApplyVisuals(GameObject cardSlot, Color cardColor, Color heldColor, Color notHeldColor)
    {
        cardSlot.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().text = cardName;
        cardSlot.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = cardDes;
        cardSlot.transform.GetChild(0).GetComponent<Image>().sprite = cardSprite;

        cardSlot.transform.GetChild(4).gameObject.SetActive(!isHeld);
        cardSlot.transform.GetChild(0).GetComponent<Image>().color = isHeld ? heldColor : notHeldColor;

        cardSlot.GetComponent<Image>().color = cardColor;
    }
}

[System.Serializable]
public class CardCondition
{
    public ConditionType conditionType; // 定义条件类型
    public int requiredValue; // 达成目标的数量或时长
    public string requiredItem; // 所需道具的ID（用于拾取类目标）
    
    public bool Evaluate(PlayerProgress progress)
    {
        switch (conditionType)
        {
            case ConditionType.GameEntries:
                return progress.gameEntries >= requiredValue;
            case ConditionType.ItemCollected:
                return progress.HasItem(requiredItem);
            default:
                return false;
        }
    }
}

// 获得成就的条件枚举
public enum ConditionType
{
    GameEntries,
    ItemCollected
}

// 保存当前进度的结构
public class PlayerProgress
{
    public int gameEntries; // 进入游戏的次数
    public HashSet<string> inventory; // 玩家已收集的物品

    public bool HasItem(string item)
    {
        return inventory.Contains(item);
    }

    public void IncrementGameEntries()
    {
        gameEntries++;
    }

    public void AddItem(string item)
    {
        inventory.Add(item);
    }
}