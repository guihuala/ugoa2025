using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIContainer : MonoBehaviour
{
    [Header("组件配置")] public Transform itemContainer;
    public GameObject itemPrefab;

    [Header("道具列表")] public List<ItemData> items = new List<ItemData>();

    private void Start()
    {
        UpdateAllUI();

        EVENTMGR.OnUnlockItem += UpdateUI;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnUnlockItem -= UpdateUI;
    }

    public void UpdateUI(ItemData item)
    {
        if (item.isUnlocked)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer);
            newItem.GetComponent<ItemUI>().UpdateItemInfo(item);
        }
    }

    public void UpdateAllUI()
    {
        foreach (var item in items)
        {
            if (item.CheckUnlock())
            {
                GameObject newItem = Instantiate(itemPrefab, itemContainer);
                newItem.GetComponent<ItemUI>().UpdateItemInfo(item);
            }
        }
    }
}