using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// SaveManager用于在游戏中进行存取档
/// 由于游戏流程是线性的，优先考虑在游戏内自动存档
/// 自动存档的规则是碰到某些地块和通关时
/// 直接覆盖当前正在游玩的存档
/// 下次点击继续游戏就可以读取存档恢复上次游戏的进度
/// 当然这样是否可行还需要经过测试
/// </summary>
public class SaveManager : SingletonPersistent<SaveManager>
{
    // 一些需要保存零散的数据
    public SceneName scensName = SceneName.Title; // 玩家上一次所在的场景，在游戏时需要触发更新
    public int playerStep = 5;
    public Vector3 playerPosition;
    public float gameTime; // 游戏时间

    // 其他需要保存的数据例如成就达成度等
    // 直接从持久化的成就管理器中获取

    public class SaveData
    {
        public SceneName scensName;
        public int playerStep;
        public Vector3 playerPosition;
        public float gameTime;

        // 达成的成就
        public List<AchievementSaveData> achievements = new List<AchievementSaveData>();

        // 新增：关卡解锁状态
        public List<LevelUnlockData> levelUnlocks = new List<LevelUnlockData>();
    }

    // 关卡解锁状态数据结构
    [System.Serializable]
    public class LevelUnlockData
    {
        public string levelName; // 关卡名称或ID
        public bool isUnlocked; // 是否已解锁
    }


    // 成就保存数据结构
    [System.Serializable]
    public class AchievementSaveData
    {
        public string cardID; // 成就卡片 ID
        public bool isHeld; // 是否已解锁
    }

    SaveData ForSave()
    {
        var savedata = new SaveData
        {
            scensName = scensName,
            playerPosition = playerPosition,
            playerStep = playerStep,
            gameTime = gameTime,
        };

        // 保存成就数据
        foreach (var achievement in AchievementManager.Instance._achievementList)
        {
            savedata.achievements.Add(new AchievementSaveData
            {
                cardID = achievement.cardID,
                isHeld = achievement.isHeld
            });
        }

        // 保存关卡解锁状态
        foreach (var level in LevelManager.Instance.levels)
        {
            savedata.levelUnlocks.Add(new LevelUnlockData
            {
                levelName = level.name,
                isUnlocked = level.isUnlocked
            });
        }

        return savedata;
    }


    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        playerPosition = savedata.playerPosition;
        playerStep = savedata.playerStep;
        gameTime = savedata.gameTime;

        // 加载成就数据
        if (savedata.achievements != null)
        {
            foreach (var achievementData in savedata.achievements)
            {
                var achievement = AchievementManager.Instance._achievementList.Find(a => a.cardID == achievementData.cardID);
                if (achievement != null)
                {
                    achievement.isHeld = achievementData.isHeld;
                }
            }
        }

        // 加载关卡解锁状态
        if (savedata.levelUnlocks != null)
        {
            foreach (var levelData in savedata.levelUnlocks)
            {
                var level = LevelManager.Instance.levels.Find(l => l.name == levelData.levelName);
                if (level != null)
                {
                    level.isUnlocked = levelData.isUnlocked;
                }
            }
        }
    }


    public void NewRecord(int ID = 0, string end = ".auto")
    {
        // 如果原位置有存档则删除
        if (RecordData.Instance.recordName[ID] != "")
        {
            DeleteRecord(ID);
        }

        // 创建新存档
        RecordData.Instance.recordName[ID] = $"{System.DateTime.Now:yyyyMMdd_HHmmss}{end}";
        RecordData.Instance.lastID = ID;
        RecordData.Instance.Save();

        Save(ID);
        SAVE.CameraCapture(ID, Camera.main, new Rect(0, 0, Screen.width, Screen.height));
    }

    void DeleteRecord(int i, bool isCover = true)
    {
        if (i < 0 || i >= RecordData.recordNum || RecordData.Instance.recordName[i] == "")
        {
            Debug.LogWarning("删除存档失败：非法的存档索引！");
            return;
        }

        Delete(i);
        RecordData.Instance.Delete();

        if (!isCover)
        {
            RecordData.Instance.recordName[i] = "";
            SAVE.DeleteShot(i);
        }
    }


    public void Save(int id)
    {
        SAVE.JsonSave(RecordData.Instance.recordName[id], ForSave());
    }

    public void Load(int id)
    {
        var saveData = SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
        ForLoad(saveData);
    }

    public SaveData ReadForShow(int id)
    {
        return SAVE.JsonLoad<SaveData>(RecordData.Instance.recordName[id]);
    }

    public void Delete(int id)
    {
        SAVE.JsonDelete(RecordData.Instance.recordName[id]);
    }
}