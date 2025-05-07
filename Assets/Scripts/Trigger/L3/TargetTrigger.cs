using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTrigger : MonoBehaviour
{
    [SerializeField] private DialogueTrigger dialogueTrigger;
    [SerializeField] private GameObject target;

    private bool isTriggered = false;

    private void Start()
    {
        EVENTMGR.OnDialogueEnd += SetPlayerTarget;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnDialogueEnd -= SetPlayerTarget;
    }

    private void SetPlayerTarget()
    {
        if (isTriggered) return;

        isTriggered = true;
        EVENTMGR.TriggerEnterTargetField(target.transform.position);
    }
}