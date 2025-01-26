using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AchievementListSO", menuName = "Achievement/AchievementListSO")]
public class AchievementList : ScriptableObject
{
    public List<AchievementSO> achievement;
}
