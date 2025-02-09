using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    // 游戏默认的成就列表
    public AchievementList DefaultAchievementList;

    // 成就系统所解锁的成就列表
    public List<AchievementSO> _achievementList;
    private HashSet<string> unlockedCards = new HashSet<string>();
    
    // 临时堆栈存储待保存的成就
    public HashSet<string> pendingAchievements = new HashSet<string>();

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
                AchievementSO clone = defaultAchievement.Clone();
                _achievementList.Add(clone);
            }
        }

        unlockedCards.Clear();
        foreach (var achievement in _achievementList)
        {
            if (achievement.isHeld)
            {
                unlockedCards.Add(achievement.ID);
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
            var achievement = _achievementList.Find(a => a.ID == savedAchievement.cardID);
            if (achievement != null)
            {
                achievement.isHeld = savedAchievement.isHeld;
                if (achievement.isHeld)
                {
                    unlockedCards.Add(achievement.ID);
                }
            }
        }
    }

    // 尝试解锁
    public void TryUnlockCards(string itemId)
    {
        foreach (var card in _achievementList) // 遍历成就列表
        {
            CheckAndUnlockCard(card, itemId);
        }
    }

    // 检查卡牌是否满足解锁的条件
    public void CheckAndUnlockCard(AchievementSO achievement, string itemId)
    {
        if (achievement.CheckCondition(itemId))
        {
            // 添加到临时堆栈
            pendingAchievements.Add(achievement.ID);
        }
    }

    public Sprite GetAchievementIcon(string itemId)
    {
        foreach (var card in _achievementList) // 遍历成就列表
        {
            if(card.ID == itemId)
                return card.icon;
        }

        return null;
    }

    // 保存成就
    public void SaveAchievements()
    {
        foreach (var cardID in pendingAchievements)
        {
            var achievement = _achievementList.Find(a => a.ID == cardID);
            if (achievement != null && !unlockedCards.Contains(achievement.ID))
            {
                unlockedCards.Add(achievement.ID);
            }
        }

        // 将已解锁的成就加入最终成就列表
        foreach (var cardID in pendingAchievements)
        {
            var achievement = _achievementList.Find(a => a.ID == cardID);
            if (achievement != null)
            {
                achievement.isHeld = true;
            }
        }

        // 清空临时堆栈
        ClearPendingAchievements();
    }

    public void ClearPendingAchievements()
    {
        pendingAchievements.Clear();
    }
    
    public string GetAchievementCompletionRatio()
    {
        int unlockedCount = unlockedCards.Count;
        
        int totalCount = _achievementList.Count;
        
        return $"{unlockedCount}/{totalCount}";
    }
}
