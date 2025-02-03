using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementUIPrefab;
    [SerializeField] private Transform achievementCardParent;

    private void Start()
    {
        EVENTMGR.OnCollectItem += ShowUnlockedAchievements;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnCollectItem -= ShowUnlockedAchievements;
    }

    // 显示所有已解锁的成就
    public void ShowUnlockedAchievements(string achievementId)
    {
        foreach (var achievement in AchievementManager.Instance._achievementList)
        {
            if (achievement.ID == achievementId)
            {
                // 创建一个新的卡片来展示成就
                GameObject newCard = Instantiate(achievementUIPrefab, achievementCardParent);
                AchievementDisplayUI cardUI = newCard.GetComponent<AchievementDisplayUI>();
                if (cardUI != null)
                {
                    // 更新卡片UI，例如显示卡片的名称、图标等
                    cardUI.SetupCard(achievement);
                }
            }
        }
    }
}