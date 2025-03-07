using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelTrigger : MonoBehaviour ,IEnterSpecialItem
{
    public string PanelName;
    
    public void Apply()
    {
        UIManager.Instance.OpenPanel(PanelName);
    }
}
