using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


[System.Serializable]
public class LevelData
{
    public string name; // 关卡名称
    public bool isUnlocked; // 是否解锁
    public bool isPlayed;
}

public class LevelManager : SingletonPersistent<LevelManager>
{
    public List<LevelData> levels = new List<LevelData>();
    public string failureReason;
    
    [Header("任务配置")] 
    public List<MissionData> missions = new List<MissionData>();

    private void Start()
    {
        // 初始化默认关卡数据
        InitLevelUnlocks();
    }

    #region 关卡管理

    public void InitLevelUnlocks()
    {
        levels = new List<LevelData>
        {
            new LevelData { name = "Level1", isUnlocked = true, isPlayed = false}, // 默认解锁第一个关卡
            new LevelData { name = "Level2", isUnlocked = false, isPlayed = false },
            new LevelData { name = "Level3", isUnlocked = false, isPlayed = false }
        };
        
        InitMissions();
    }

    // 从存档读取数据
    public void LoadLevelUnlocks(int ID)
    {
        var saveData = SaveManager.Instance.ReadForShow(ID);
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
            SaveManager.Instance.NewRecord(); // 保存解锁状态
        }
        
        // 如果是3、6关，直接用笨办法吧
        if(levelName == "Level3" || levelName == "Level6")
            UnlockMission(levelName);
    }

    // 标记关卡被游玩过
    public void PlayLevel(string levelName)
    {
        var level = levels.Find(l => l.name == levelName);
        if (level != null && !level.isPlayed)
        {
            level.isPlayed = true;
        }
    }
    
    public string GetLastUnlockedLevel(List<SaveManager.LevelUnlockData> levelDates)
    {
        for (int i = levelDates.Count - 1; i >= 0; i--)
        {
            if (levelDates[i].isUnlocked)
            {
                return $"第{i + 1}关";
            }
        }
        return "None";
    }    

    #endregion

    /// <summary>
    /// 任务的解锁条件只有关卡进度，并且也没有实际意义上的奖励，所以就放一块了
    /// </summary>
    #region 任务管理

    // 任务信息硬编码
    private void InitMissions()
    {
        missions = new List<MissionData>
        {
            new MissionData
            {
                missionName = "1", missionDescription = "", missionIcon = Resources.Load<Sprite>("Icons/Missions/"),
                isMissionUnlocked = false, isMissionAccepted = true, unlockRequiredLevel = "Level1"
            },
            new MissionData
            {
                missionName = "2", missionDescription = "", missionIcon = Resources.Load<Sprite>("Icons/Missions/"),
                isMissionUnlocked = false, isMissionAccepted = false, unlockRequiredLevel = "Level3"
            },
            new MissionData
            {
                missionName = "3", missionDescription = "", missionIcon = Resources.Load<Sprite>("Icons/Missions/"),
                isMissionUnlocked = false, isMissionAccepted = false, unlockRequiredLevel = "Level6"
            },
        };
    }

    private void LoadMissionUnlocks()
    {
        
    }
    
    private void UnlockMission(string levelName)
    {
        var mission = missions.Find(l => l.missionName == levelName);
        if (mission != null)
        {
            mission.isMissionUnlocked = true;
        }
    }

    #endregion
}