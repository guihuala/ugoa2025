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
    public string ID;
    public string name;
    [TextArea]public string des;
    public Sprite icon;
    public bool isHeld;
    public AchievementCondition condition;

    public AchievementSO Clone()
    {
        return new AchievementSO
        {
            ID = this.ID,
            name = this.name,
            isHeld = this.isHeld,
            des = this.des,
            icon = this.icon,
            condition = this.condition,
        };
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
        if (isHeld)
        {
            cardSlot.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = name;
            cardSlot.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = des;
            cardSlot.transform.GetChild(0).GetComponent<Image>().sprite = icon;
            cardSlot.transform.GetChild(0).GetComponent<Image>().color = Color.white;
            cardSlot.transform.GetChild(3).gameObject.SetActive(!isHeld);
        }
        else
        {
            cardSlot.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().text = "???";
            cardSlot.transform.GetChild(1).transform.GetChild(0).GetComponent<Text>().text = "??????";
            cardSlot.transform.GetChild(0).GetComponent<Image>().sprite = icon;
            cardSlot.transform.GetChild(0).GetComponent<Image>().color = Color.gray;
            cardSlot.transform.GetChild(3).gameObject.SetActive(!isHeld);
        }
    }
}


// 定义卡牌解锁条件的类
[System.Serializable]
public class AchievementCondition
{
    public string requiredItem; // 达成目标所需的道具 ID
}