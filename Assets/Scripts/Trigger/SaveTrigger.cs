using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveTrigger : MonoBehaviour ,IEnterSpecialItem
{
    
    public void Apply()
    {
        UIManager.Instance.OpenPanel("SavePanel");
    }
}
