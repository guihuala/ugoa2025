using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionData
{
    public string missionName;
    [TextArea] public string missionDescription;
    public Sprite missionIcon;

    public bool isMissionUnlocked;
    public bool isMissionAccepted;
    
    public string unlockRequiredLevel;
}