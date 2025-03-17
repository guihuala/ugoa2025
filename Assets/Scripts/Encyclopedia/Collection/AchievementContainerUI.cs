using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchievementContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject achievementUIPrefab;
    [SerializeField] private Transform achievementCardParent;
    [SerializeField] private Vector3 cardScaleIn = new Vector3(0f, 0f, 1f);  // 挤压效果的目标大小
    [SerializeField] private Vector3 cardScaleNormal = new Vector3(1f, 1f, 1f); // 正常大小

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
                    cardUI.SetupCard(achievement);
                }

                // 启动挤压动画
                StartCoroutine(AnimateCard(newCard));
            }
        }
    }

    private IEnumerator AnimateCard(GameObject card)
    {
        RectTransform rectTransform = card.GetComponent<RectTransform>();
        
        rectTransform.localScale = cardScaleIn;
        
        float elapsed = 0f;
        float duration = 0.2f;
        while (elapsed < duration)
        {
            rectTransform.localScale = Vector3.Lerp(cardScaleIn, cardScaleNormal, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        rectTransform.localScale = cardScaleNormal;
    }
}
