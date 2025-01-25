using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AchievementManager : SingletonPersistent<AchievementManager>
{
    public List<CardSO> cards = new List<CardSO>();
    private HashSet<string> unlockedCards;
    private PlayerProgress playerProgress;

    private void Start()
    {
        LoadUnlockedCards();
        LoadPlayerProgress();
        InvokeRepeating(nameof(SavePlayerProgress), 60f, 60f); // 每分钟保存一次
    }
    
    public void TryUnlockCards(PlayerProgress progress)
    {
        foreach (var card in cards)
        {
            // 如果卡牌未解锁且满足条件，则解锁卡牌
            if (!IsCardUnlocked(card) && card.CheckCondition(progress))
            {
                CheckAndUnlockCard(card, progress);
                if (IsCardUnlocked(card))
                {
                    Debug.Log($"Card unlocked: {card.cardName}");
                }
            }
        }
    }

    public bool IsCardUnlocked(CardSO card)
    {
        return unlockedCards.Contains(card.cardID);
    }

    public void CheckAndUnlockCard(CardSO card, PlayerProgress progress)
    {
        if (unlockedCards.Contains(card.cardID)) return;

        if (card.CheckCondition(progress))
        {
            unlockedCards.Add(card.cardID);
            card.isHeld = true;

            SaveUnlockedCards(); // 立即保存解锁状态
        }
    }

    private void LoadUnlockedCards()
    {
        // unlockedCards = new HashSet<string>();
        //
        //
        // string savedData = PlayerPrefs.GetString("UnlockedCards", "");
        // if (!string.IsNullOrEmpty(savedData))
        // {
        //     try
        //     {
        //         var cardDataList = JsonConvert.DeserializeObject<List<CardData>>(savedData);
        //         foreach (var cardData in cardDataList)
        //         {
        //             unlockedCards.Add(cardData.cardID);
        //             var card = cards.Find(c => c.cardID == cardData.cardID);
        //             if (card != null)
        //             {
        //                 card.isHeld = cardData.isHeld;
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Debug.LogError("Error loading unlocked cards: " + ex.Message);
        //     }
        // }
    }

    private void SaveUnlockedCards()
    {
        // try
        // {
        //     // 创建卡牌数据列表
        //     var cardDataList = new List<CardData>();
        //     foreach (var card in cards)
        //     {
        //         cardDataList.Add(new CardData
        //         {
        //             cardID = card.cardID,
        //             isHeld = card.isHeld
        //         });
        //     }
        //
        //     // 序列化为 JSON 并保存到 PlayerPrefs
        //     string json = JsonConvert.SerializeObject(cardDataList);
        //     PlayerPrefs.SetString("UnlockedCards", json);
        //     PlayerPrefs.Save();
        // }
        // catch (Exception ex)
        // {
        //     Debug.LogError("Error saving unlocked cards: " + ex.Message);
        // }
    }

    // 加载单个成就的进度
    private void LoadPlayerProgress()
    {
        playerProgress = new PlayerProgress
        {
            gameEntries = PlayerPrefs.GetInt("Progress_GameEntries", 0),
            inventory = new HashSet<string>() // 从自定义存储加载
        };
    }

    public void ResetUnlockedCards()
    {
        unlockedCards.Clear();
        foreach (var card in cards)
        {
            card.isHeld = false;
        }
        SaveUnlockedCards();
        Debug.Log("Unlocked cards have been reset.");
    }

    // 保存进度
    public void SavePlayerProgress()
    {
        try
        {
            PlayerPrefs.SetInt("Progress_GameEntries", playerProgress.gameEntries);
            PlayerPrefs.Save();
            Debug.Log("Player progress saved.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error saving player progress: " + ex.Message);
        }
    }

    public List<CardSO> GetCardsByClass(CardClass cardClass)
    {
        return cards.FindAll(card => card.cardClass == cardClass);
    }
}