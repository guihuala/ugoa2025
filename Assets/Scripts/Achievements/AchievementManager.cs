using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    public AchievementList achievementList;
    private HashSet<string> unlockedCards = new HashSet<string>();
    public PlayerProgress playerProgress;

    public void TryUnlockCards()
    {
        foreach (var card in achievementList.achievement) // 遍历成就列表
        {
            if (!IsCardUnlocked(card) && card.CheckCondition(playerProgress))
            {
                CheckAndUnlockCard(card, playerProgress);
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
        }
    }
}