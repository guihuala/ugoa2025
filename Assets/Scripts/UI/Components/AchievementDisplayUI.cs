using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementDisplayUI : MonoBehaviour
{
    [SerializeField] private Image achievementIcon;

    public void SetupCard(AchievementSO achievement)
    {
        if (achievementIcon != null)
        {
            achievementIcon.sprite = achievement.cardSprite;
        }
    }
}