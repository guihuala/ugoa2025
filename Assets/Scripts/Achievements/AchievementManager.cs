using System;
using System.Collections;
using System.Collections.Generic;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    // 游戏默认的成就列表
    public AchievementList DefaultAchievementList;

    // 成就系统所解锁的成就列表
    public List<AchievementSO> _achievementList;
    private HashSet<string> unlockedCards = new HashSet<string>();

    private void Start()
    {
        EVENTMGR.OnCollectItem += TryUnlockCards;
        
        InitLockCards();
    }

    public void InitLockCards()
    {
        if (_achievementList == null)
        {
            _achievementList = new List<AchievementSO>();
        }

        _achievementList.Clear();

        if (DefaultAchievementList != null && DefaultAchievementList.achievement != null)
        {
            foreach (var defaultAchievement in DefaultAchievementList.achievement)
            {
                _achievementList.Add(defaultAchievement);
            }
        }

        unlockedCards.Clear();
        foreach (var achievement in _achievementList)
        {
            if (achievement.isHeld)
            {
                unlockedCards.Add(achievement.cardID);
            }
        }
    }

    // 从存档数据加载成就状态
    public void LoadAchievements(int ID)
    {
        var saveData = SaveManager.Instance.ReadForShow(ID);
        var savedAchievements = saveData.achievements;
        
        foreach (var savedAchievement in savedAchievements)
        {
            var achievement = _achievementList.Find(a => a.cardID == savedAchievement.cardID);
            if (achievement != null)
            {
                achievement.isHeld = savedAchievement.isHeld;
                if (achievement.isHeld)
                {
                    unlockedCards.Add(achievement.cardID);
                }
            }
        }
    }

    // 尝试解锁卡牌
    public void TryUnlockCards(string itemId)
    {
        foreach (var card in DefaultAchievementList.achievement) // 遍历成就列表
        {
            CheckAndUnlockCard(card,itemId);
        }
    }

    // 查询卡牌是否解锁
    public bool IsCardUnlocked(AchievementSO achievement)
    {
        return unlockedCards.Contains(achievement.cardID);
    }

    // 检查卡牌是否满足解锁的条件
    public void CheckAndUnlockCard(AchievementSO achievement,string itemId)
    {
        if (unlockedCards.Contains(achievement.cardID)) return;

        if (achievement.CheckCondition(itemId))
        {
            // 添加已经解锁的卡牌
            unlockedCards.Add(achievement.cardID);
            achievement.isHeld = true;
        }
    }
}