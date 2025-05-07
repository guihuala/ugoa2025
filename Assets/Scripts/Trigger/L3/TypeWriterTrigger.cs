using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TypeWriterTrigger : MonoBehaviour,IEnterSpecialItem
{
    private bool isActive = false;

    private void Start()
    {
        EVENTMGR.OnDialogueEnd += OpenTypeWriterPanel;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnDialogueEnd -= OpenTypeWriterPanel;
    }

    public void Apply()
    {
        isActive = true;
    }

    private void OpenTypeWriterPanel()
    {
        if(!isActive)return;

        UIManager.Instance.OpenPanel("BlackTypeWriter");
    }
}
