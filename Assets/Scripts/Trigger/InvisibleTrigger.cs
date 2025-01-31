using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvisibleTrigger : MonoBehaviour,IEnterSpecialItem,IExitSpecialItem
{
    // 灌木丛地块
    
    public void Apply()
    { 
        EVENTMGR.TriggerStepIntoGrass(true);
    }

    public void UnApply()
    {
        EVENTMGR.TriggerStepIntoGrass(false);
    }
}
