using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class LevelData
{
    public string name; // 关卡名称
    public bool isUnlocked; // 是否解锁
}

public class LevelManager : SingletonPersistent<LevelManager>
{
    public List<LevelData> levels = new List<LevelData>();

    private void Start()
    {
        // 初始化默认关卡数据
        InitLevelUnlocks();
    }

    public void InitLevelUnlocks()
    {
        levels = new List<LevelData>
        {
            new LevelData { name = "Level1", isUnlocked = true }, // 默认解锁第一个关卡
            new LevelData { name = "Level2", isUnlocked = false },
            new LevelData { name = "Level3", isUnlocked = false }
        };
    }

    // 从存档读取数据
    public void LoadLevelUnlocks()
    {
        var saveData = SaveManager.Instance.ReadForShow(0);
        if (saveData != null && saveData.levelUnlocks != null)
        {
            foreach (var levelData in saveData.levelUnlocks)
            {
                var level = levels.Find(l => l.name == levelData.levelName);
                if (level != null)
                {
                    level.isUnlocked = levelData.isUnlocked;
                }
            }
        }
    }

    // 解锁关卡
    public void UnlockLevel(string levelName)
    {
        var level = levels.Find(l => l.name == levelName);
        if (level != null && !level.isUnlocked)
        {
            level.isUnlocked = true;
            Debug.Log($"Level {levelName} unlocked.");
            SaveManager.Instance.Save(0); // 保存解锁状态
        }
    }
}