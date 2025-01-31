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
        
        totalNumbers = AchievementManager.Instance.DefaultAchievementList.achievement.Count;

        UpdateMaxPage();
        UpdatePageUI(); 
        DisplayCards(page);
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

    private void AnimateCard(GameObject cardSlot)
    {
        cardSlot.transform.localScale = Vector3.zero;
        Vector3 startPosition = cardSlot.transform.localPosition;
        cardSlot.transform.localPosition = startPosition - new Vector3(0, 50f, 0); // 初始位置下移
    
        cardSlot.SetActive(true);

        // 透明度渐入
        CanvasGroup canvasGroup = cardSlot.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = cardSlot.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1, 0.5f);

        // 位移 + 缩放进入
        cardSlot.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
        cardSlot.transform.DOLocalMoveY(startPosition.y, 0.5f).SetEase(Ease.OutQuad);

        // 轻微弹跳
        cardSlot.transform.DOPunchScale(Vector3.one * 0.05f, 0.3f, 8, 0.5f).SetDelay(0.5f);
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
            page = maxPage; // 循环到最后一页
        }

        DisplayCards(page);
        UpdatePageUI();
    }
}