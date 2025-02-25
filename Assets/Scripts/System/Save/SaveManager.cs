using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveManager : SingletonPersistent<SaveManager>
{
    // 一些需要保存零散的数据
    public SceneName scensName = SceneName.Title; // 玩家上一次所在的场景，在游戏时需要触发更新
    public float gameTime; // 游戏时间
    public bool isComplete = false;

    public int ID;

    // 其他需要保存的数据例如成就达成度等
    // 直接从持久化的成就管理器中获取

    private void Update()
    {
        TIMEMGR.SetCurTime();
    }

    public class SaveData
    {
        public SceneName scensName;
        public float gameTime;
        public bool isComplete;

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

    public void SetDefaultCurrentScene()
    {
        SceneName sceneName = SceneName.LevelSelection;
    }

    SaveData ForSave()
    {
        AchievementManager.Instance.SaveAchievements();

        var savedata = new SaveData
        {
            scensName = scensName,
            gameTime = gameTime,
            isComplete = isComplete
        };

        // 保存成就数据
        foreach (var achievement in AchievementManager.Instance._achievementList)
        {
            savedata.achievements.Add(new AchievementSaveData
            {
                cardID = achievement.ID,
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
        gameTime = savedata.gameTime;
        isComplete = savedata.isComplete;
    }


    public void NewRecord(string end = ".save")
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
        
        TIMEMGR.SetOriTime();
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