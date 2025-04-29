using System;
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
    public bool requiresItems; // 是否需要物品解锁
    public List<string> requiredItemIDs; // 需要的物品ID列表
}

public class LevelManager : SingletonPersistent<LevelManager>
{
    [Header("关卡配置")]
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
            new LevelData
            {
                name = "Level1_1",
                isUnlocked = true,
                isPlayed = false,
                requiresItems = false
            },
            new LevelData { 
                name = "Level1_2",
                isUnlocked = false,
                isPlayed = false,
                requiresItems = false
                
            },
            new LevelData
            {
                name = "Level1_3", 
                isUnlocked = false,
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level1_4",
                isUnlocked = false,
                isPlayed = false,
                requiresItems = true,
                requiredItemIDs = new List<string> { "1", "2", "3", "4" }
            },

            new LevelData
            {
                name = "Level2_1",
                isUnlocked = false, 
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level2_2",
                isUnlocked = false, 
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level2_3", 
                isUnlocked = false, 
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level2_4", 
                isUnlocked = false,
                isPlayed = false,
                requiresItems = true,
                requiredItemIDs = new List<string> { "5", "6", "7", "8", "9", "10" }
            },

            new LevelData
            {
                name = "Level3_1", 
                isUnlocked = false, 
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level3_2",
                isUnlocked = false,
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level3_3",
                isUnlocked = false, 
                isPlayed = false,
                requiresItems = false
            },
            new LevelData
            {
                name = "Level3_4",
                isUnlocked = false,
                isPlayed = false,
                requiresItems = true,
                requiredItemIDs = new List<string> { "11", "12", "13", "14", "15" }
            },
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
        
        LoadMissionUnlocks(saveData);
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
        
        // 如果是每个主题的底关就解锁任务，直接用笨办法吧
        if(levelName == "Level1_3" || levelName == "Level2_3")
            UnlockMission(levelName);
    }

    public void UnlockSpecialLevel(LevelData levelData)
    {
        // 检查所有需要的物品
        foreach (var itemID in levelData.requiredItemIDs)
        {
            if (!AchievementManager.Instance.CheckUnlockCard(itemID))
            {
                return;
            }
        }

        if (!levelData.isUnlocked)
        {
            levelData.isUnlocked = true;
            SaveManager.Instance.NewRecord(); // 保存解锁状态
        }
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
                string levelName = levelDates[i].levelName;
                
                int startIndex = Math.Max(0, levelName.Length - 3);
                return $"第{levelName.Substring(startIndex)}关";
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
                missionID = "1",
                missionTitle = "婚礼委托",
                missionDescription =
                    "护林员女士：\n近日接获报告，雨林东北区的部落即将举行一场重要婚礼，局里决定派你作为代表出席。按照传统，请务必携带礼物。\n注意：巡逻队在该区域发现了盗猎者设置的陷阱，目标疑似珍稀鸟兽。若遭遇，请避免正面冲突，优先记录证据。\n祝任务顺利。\n——林业局后勤科",
                missionIcon = Resources.Load<Sprite>("Icons/Missions/Mission1"),
                isMissionUnlocked = true, isMissionAccepted = false, unlockRequiredLevel = "Level1_1"
            },
            new MissionData
            {
                missionID = "2",
                missionTitle = "民俗学家的委托",
                missionDescription =
                    "护林员女士：\n我在研究一个即将消失的古老习俗——‘白鹤共生祭’。根据零散的记载，黑土平原深处可能还存在举行该仪式的村落。\n委托内容：\n1. 找到祭祀村落并拍摄白鹤群舞的照片（附：旧版地图与胶卷）\n2. 若可能，记录祭祀歌谣的歌词（我的录音笔电量不足了…）\n警告：当地老人提过，平原上有位养鼠的巫婆，遇到她请保持距离。\n\n——（民俗学家的名字）（随信附赠一包驱鼠药粉）",
                missionIcon = Resources.Load<Sprite>("Icons/Missions/"),
                isMissionUnlocked = false, isMissionAccepted = false, unlockRequiredLevel = "Level2_1"
            },
            new MissionData
            {
                missionID = "3",
                missionTitle = "金字塔调查",
                missionDescription =
                    "紧急通知：\n金字塔群发生多起‘木乃伊目击事件’。初步调查显示，盗墓者破坏了内部祭坛，导致封印失效。\n你的任务：\n1. 查明盗墓者动机与行踪\n2. 收集散落的3件祭品（清单附图）\n3. 恢复金字塔封印（操作指南见附件）\n\n——文物局危机应对科",
                missionIcon = Resources.Load<Sprite>("Icons/Missions/"),
                isMissionUnlocked = false, isMissionAccepted = false, unlockRequiredLevel = "Level3_1"
            },
        };
    }

    private void LoadMissionUnlocks(SaveManager.SaveData saveData)
    {
        if (saveData != null && saveData.levelUnlocks != null)
        {
            foreach (var missionData in saveData.missions)
            {
                var mission = missions.Find(m => m.missionID == missionData.missionID);
                if (mission != null)
                {
                    mission.isMissionUnlocked = missionData.isUnlocked;
                    mission.isMissionAccepted = missionData.isUnlocked;
                }
            }
        }
    }
    
    private void UnlockMission(string levelName)
    {
        var mission = missions.Find(l => l.missionID == levelName);
        if (mission != null)
        {
            mission.isMissionUnlocked = true;
        }
    }

    // 与对应的任务的信件互动即为接受
    public void AcceptMission(string levelName)
    {
        var mission = missions.Find(l => l.missionID == levelName);
        if (mission != null)
        {
            mission.isMissionAccepted = true;
        }
    }

    #endregion
}