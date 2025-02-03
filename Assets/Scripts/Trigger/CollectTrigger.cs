using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // 导入DOTween命名空间

public class CollectTrigger : MonoBehaviour, IEnterSpecialItem
{
    public string itemID; // 物品唯一标识符

    private Transform itemSprite;
    private bool isCollected = false; // 是否已被拾取
    
    private float floatAmount = 0.2f; // 浮动的最大高度
    private float floatDuration = 1f; // 浮动一个周期所用的时间
    
    private void Awake()
    {
        itemSprite = transform.GetChild(0);
        
        ApplyFloatingEffect();
    }

    public void Apply()
    {
        if (isCollected)
        {
            return;
        }
        
        isCollected = true;

        EVENTMGR.TriggerCollectItem(itemID);

        Destroy(gameObject);
    }
    
    private void ApplyFloatingEffect()
    {
        if (itemSprite != null)
        {
            itemSprite.DOLocalMoveY(itemSprite.localPosition.y + floatAmount, floatDuration)
                .SetLoops(-1, LoopType.Yoyo)
                .SetEase(Ease.InOutSine);
        }
    }
}