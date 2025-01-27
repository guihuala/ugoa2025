using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectTrigger : MonoBehaviour, IEnterSpecialItem
{
    public string itemID; // 物品唯一标识符

    private bool isCollected = false; // 是否已被拾取

    // 拾取后触发的事件
    protected virtual void OnItemCollected()
    {

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
        
        EVENTMGR.TriggerCollectItem(itemID);
        
        Destroy(gameObject);
    }
}