using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 定义可拾取物品的基类
/// </summary>
public class CollectTrigger : MonoBehaviour, ICollectible
{
    public string itemID; // 物品唯一标识符
    public string itemName; // 物品名称

    private bool isCollected; // 是否已被拾取
    private ICollectible _collectibleImplementation;

    // 拾取后触发的事件
    protected virtual void OnItemCollected(PlayerProgress playerProgress)
    {
        
    }

    public void OnCollect()
    {
        if (isCollected)
        {
            Debug.LogWarning($"{itemName} 已经被拾取，无法重复拾取！");
            return;
        }

        // 将物品添加到玩家进度中
        AchievementManager.Instance.playerProgress.AddItem(itemID);

        // 更新物品状态
        isCollected = true;

        // 触发拾取逻辑，例如更新成就（此处占位）
        OnItemCollected(AchievementManager.Instance.playerProgress);
        
        AchievementManager.Instance.TryUnlockCards();

        // 隐藏或销毁物品
        gameObject.SetActive(false);
        Debug.Log($"{itemName} 已被拾取！");
    }
}