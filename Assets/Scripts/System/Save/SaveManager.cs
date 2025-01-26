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
    public SceneName scensName = SceneName.MainMenu; // 玩家上一次所在的场景，在游戏时需要触发更新
    // 玩家保存的脚步
    public float gameTime; // 游戏时间

    // 其他需要保存的数据例如成就达成度等
    // 直接从持久化的成就管理器中获取

    public class SaveData
    {
        public SceneName scensName;
        public float gameTime;

        // 达成的成就、当前进度
        public List<AchievementSaveData> achievements = new List<AchievementSaveData>();
        public PlayerProgress playerProgress = new PlayerProgress();
    }

    // 成就保存数据结构
    [System.Serializable]
    public class AchievementSaveData
    {
        public string cardID;  // 成就卡片 ID
        public bool isHeld;    // 是否已解锁
    }

    SaveData ForSave()
    {
        var savedata = new SaveData
        {
            scensName = scensName,
            gameTime = gameTime,
            playerProgress = AchievementManager.Instance.playerProgress
        };

        // 保存成就数据
        foreach (var achievement in AchievementManager.Instance.achievementList.achievement)
        {
            savedata.achievements.Add(new AchievementSaveData
            {
                cardID = achievement.cardID,
                isHeld = achievement.isHeld
            });
        }

        return savedata;
    }

    void ForLoad(SaveData savedata)
    {
        scensName = savedata.scensName;
        gameTime = savedata.gameTime;
        AchievementManager.Instance.playerProgress = savedata.playerProgress;

        // 加载成就数据
        if (savedata.achievements != null)
        {
            foreach (var achievementData in savedata.achievements)
            {
                var achievement = AchievementManager.Instance.achievementList.achievement.Find(a => a.cardID == achievementData.cardID);
                if (achievement != null)
                {
                    achievement.isHeld = achievementData.isHeld;
                }
            }
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