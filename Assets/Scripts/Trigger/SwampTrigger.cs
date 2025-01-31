using System.Collections.Generic;
using UnityEngine;

public class SwampTrigger : MonoBehaviour, IEnterSpecialItem, IStayInSpecialItem, IExitSpecialItem
{
    public void Apply()
    {
        EVENTMGR.TriggerEnterSwamp();
    }

    public void Stay(float stayDuration)
    {
        EVENTMGR.TriggerStayInSwamp(stayDuration);
    }

    public void UnApply()
    {
        EVENTMGR.TriggerExitSwamp();
    }
}