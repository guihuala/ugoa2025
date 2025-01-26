using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Serialization;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    public AchievementList achievementList;
    private HashSet<string> unlockedCards = new HashSet<string>();
    public PlayerProgress playerProgress;

    public void TryUnlockCards(PlayerProgress progress)
    {
        foreach (var card in achievementList.achievement) // 遍历成就列表
        {
            if (!IsCardUnlocked(card) && card.CheckCondition(progress))
            {
                CheckAndUnlockCard(card, progress);
            }
        }
    }

    public bool IsCardUnlocked(AchievementSO achievement)
    {
        return unlockedCards.Contains(achievement.cardID);
    }

    public void CheckAndUnlockCard(AchievementSO achievement, PlayerProgress progress)
    {
        if (unlockedCards.Contains(achievement.cardID)) return;

        if (achievement.CheckCondition(progress))
        {
            unlockedCards.Add(achievement.cardID);
            achievement.isHeld = true;
            // 可以自动保存一下
        }
    }
}