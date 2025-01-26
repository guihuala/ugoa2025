using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    // 游戏默认的成就列表
    public AchievementList DefaultAchievementList;
    // 成就系统所解锁的成就列表
    public AchievementList _achievementList;
    private HashSet<string> unlockedCards = new HashSet<string>();
    public PlayerProgress playerProgress;

    private void Start()
    {
        
    }

    public void InitLockCards()
    {
        _achievementList = DefaultAchievementList;
    }
    
    // 尝试解锁卡牌
    public void TryUnlockCards()
    {
        foreach (var card in DefaultAchievementList.achievement) // 遍历成就列表
        {
            if (!IsCardUnlocked(card) && card.CheckCondition(playerProgress))
            {
                CheckAndUnlockCard(card, playerProgress);
            }
        }
    }

    // 查询卡牌是否解锁
    public bool IsCardUnlocked(AchievementSO achievement)
    {
        return unlockedCards.Contains(achievement.cardID);
    }

    // 检查卡牌是否满足解锁的条件
    public void CheckAndUnlockCard(AchievementSO achievement, PlayerProgress progress)
    {
        if (unlockedCards.Contains(achievement.cardID)) return;

        if (achievement.CheckCondition(progress))
        {
            // 添加已经解锁的卡牌
            unlockedCards.Add(achievement.cardID);
            achievement.isHeld = true;
        }
    }
}