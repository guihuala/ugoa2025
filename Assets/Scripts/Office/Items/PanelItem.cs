using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelItem : BaseOfficeItem
{
    public string panelName;

    protected override void Apply()
    {
        base.Apply();

        UIManager.Instance.OpenPanel(panelName);
    }
}
