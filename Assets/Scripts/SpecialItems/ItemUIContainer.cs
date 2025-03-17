using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemUIContainer : MonoBehaviour
{
    [Header("组件配置")]
    public Transform itemContainer;
    public GameObject itemPrefab;
    
    [Header("道具列表")]
    public List<ItemData> items = new List<ItemData>();

    private void Start()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        foreach (var item in items)
        {
            if (item.CheckUnlock(LevelManager.Instance.levels))
            {
                GameObject newItem = Instantiate(itemPrefab, itemContainer);
                newItem.GetComponent<ItemUI>().UpdateItemInfo(item);
            }
        }
    }
}
