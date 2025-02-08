using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class InvisibleTrigger : MonoBehaviour,IEnterSpecialItem
{
    // 灌木丛地块
    
    public void Apply()
    { 
        EVENTMGR.TriggerStepIntoGrass(true);
    }
}
