using System.Collections.Generic;
using UnityEngine;

public class SwampTrigger : MonoBehaviour, IEnterSpecialItem
{
    public void Apply()
    {
        EVENTMGR.TriggerEnterSwamp();
    }
}