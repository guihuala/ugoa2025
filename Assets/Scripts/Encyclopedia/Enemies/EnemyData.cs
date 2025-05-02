using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EnemyInfo", menuName = "Encyclopedia/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    [TextArea]public string enemyDescription;
    public Sprite enemySprite;
    
    public string requiredLevelName; // 该敌人解锁所需的关卡名称
    public bool isUnlocked;    // 是否解锁

    // 判断敌人是否解锁
    public bool CheckUnlock(List<LevelData> levels)
    {
        LevelData requiredLevel = levels.Find(l => l.name == requiredLevelName);
        
        if (requiredLevel != null && requiredLevel.isUnlocked && requiredLevel.isPlayed)
        {
            
            isUnlocked = true;
        }
        else
        {
            isUnlocked = false;
        }

        return isUnlocked;
    }
}
