using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ItemTrigger : MonoBehaviour
{
    public ItemData itemData;

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
        
        AudioManager.Instance.PlaySfx("Collect_2");
        
        itemData.UnlockItem();

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
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Apply();
        }
    }
}
