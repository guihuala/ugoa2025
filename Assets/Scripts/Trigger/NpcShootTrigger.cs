using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcShootTrigger : MonoBehaviour ,IShootable
{
    private Sprite[] sprites;
    
    public void OnShot(BulletLifecycle bullet)
    {
        EVENTMGR.TriggerPlayerFound();
    }
}
