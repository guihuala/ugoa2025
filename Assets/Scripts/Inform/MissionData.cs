using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewMission", menuName = "Mission Data")]
public class MissionData : ScriptableObject
{
    public string missionName;
    [TextArea] public string missionDescription;
    public Sprite missionIcon;
    public bool isMissionAccepted;
}