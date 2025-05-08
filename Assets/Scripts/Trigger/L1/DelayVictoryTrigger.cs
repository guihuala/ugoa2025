using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayVictoryTrigger : MonoBehaviour, IEnterSpecialItem
{
    private bool isTriggered = false;
    private bool isActive = false;

    private void Start()
    {
        EVENTMGR.OnDialogueEnd += OpenVictoryPanel;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnDialogueEnd -= OpenVictoryPanel;
    }

    private void OpenVictoryPanel()
    {
        if (isTriggered) return;
        if (!isActive) return;

        isTriggered = true;
        UIManager.Instance.OpenPanel("GameVictoryPanel");
    }
    
    public void Apply()
    {
        isActive = true;
    }
}
