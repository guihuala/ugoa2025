using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightItem : BaseOfficeItem
{
    public Light light;

    protected override void Start()
    {
        base.Start();
        
        light.enabled = false;
    }

    protected override void Apply()
    {
        base.Apply();
        
        light.enabled = !light.enabled;
    }
}
