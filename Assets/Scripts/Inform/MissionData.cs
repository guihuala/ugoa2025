using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class MissionData
{
    public string missionID;
    
    public string missionTitle;
    [TextArea] public string missionDescription;
    public Sprite missionIcon;

    public bool isMissionUnlocked;
    public bool isMissionAccepted;
    
    public string unlockRequiredLevel;
}