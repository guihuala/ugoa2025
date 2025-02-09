using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementPanel : BasePanel
{
    public GameObject[] cardSlots;
    
    [Header("Page Settings")]
    public int page = 0;
    public int cardsPerPage = 4;
    public Text pageText;

    public Text statisticsText;
    
    private int totalNumbers; // 用于显示总卡片数量
    private int maxPage; // 用于翻页时计算最大页数

    public Button nextPageButton;
    public Button previousPageButton;
    public Button closeButton;

    private void Start()
    {
        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(() => UIManager.Instance.ClosePanel(panelName));
        
        totalNumbers = AchievementManager.Instance._achievementList.Count;

        UpdateMaxPage();
        UpdatePageUI(); 
        DisplayCards(page);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        UpdateStatisticsText();
    }

    void UpdateMaxPage()
    {
        maxPage = Mathf.CeilToInt((float)totalNumbers / cardsPerPage);
    }

    void UpdatePageUI()
    {
        pageText.text = $"{page + 1}/{maxPage}";
    }

    private void DisplayCards(int _page)
    {
        ResetCardSlots();

        List<AchievementSO> cardsToDisplay = AchievementManager.Instance._achievementList;

        for (int i = 0; i < cardsToDisplay.Count; i++)
        {
            if (i >= _page * cardsPerPage && i < (_page + 1) * cardsPerPage)
            {
                AchievementSO achievement = cardsToDisplay[i];
                GameObject slot = cardSlots[i % cardsPerPage];

                slot.gameObject.SetActive(true);
                achievement.ApplyVisuals(slot);
            }
        }
    }

    private void UpdateStatisticsText()
    {
        
        statisticsText.text = "收集进度：\n" + AchievementManager.Instance.GetAchievementCompletionRatio();
    }

    private void ResetCardSlots()
    {
        foreach (var slot in cardSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }
    
    public void NextPage()
    {
        if (page < maxPage - 1)
        {
            page++;
        }
        else
        {
            page = 0; // 循环到第一页
        }

        DisplayCards(page);
        UpdatePageUI();
    }

    public void PreviousPage()
    {
        if (page > 0)
        {
            page--;
        }
        else
        {
            page = maxPage - 1; // 循环到最后一页
        }

        DisplayCards(page);
        UpdatePageUI();
    }
}