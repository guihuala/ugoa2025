using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 定义可拾取物品的基类
/// </summary>
public class CollectTrigger : MonoBehaviour, IEnterSpecialItem
{
    public string itemID; // 物品唯一标识符
    public string itemName; // 物品名称

    private bool isCollected = false; // 是否已被拾取

    // 拾取后触发的事件
    protected virtual void OnItemCollected()
    {
        // 将物品添加到玩家进度中
        // AchievementManager.Instance.playerProgress.AddItem(itemID);
        // AchievementManager.Instance.TryUnlockCards();
    }

    public void Apply()
    {
        if (isCollected)
        {
            return;
        }
        
        // 更新物品状态
        isCollected = true;
        
        OnItemCollected();
        
        Destroy(gameObject);
        Debug.Log($"{itemName} 已被拾取！");
    }
}