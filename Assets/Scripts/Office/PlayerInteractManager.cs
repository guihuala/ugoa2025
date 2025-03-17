using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteractManager : MonoBehaviour
{
    public BaseOfficeItem officeItem; // 当前激活的物体

    private void Start()
    {
        officeItem = null;
    }

    public void UpdateOfficeItem(BaseOfficeItem newItem)
    {
        if (officeItem != null)
        {
            // 这里可以放置逻辑来禁用之前的物体，例如恢复其状态
            officeItem.HideHighlight();
        }
        
        officeItem = newItem;
    }

    public bool CheckItem(BaseOfficeItem newItem)
    {
        if (officeItem == null)
            return false;
        
        return newItem.gameObject == officeItem.gameObject;
    }
}
