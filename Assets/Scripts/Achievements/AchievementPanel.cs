using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class AchievementPanel : BasePanel
{
    public GameObject[] cardSlots;

    [Header("Card Background Color")]
    public Color heldColor;
    public Color notHeldColor;
    
    [Header("Page Settings")]
    public int page = 0;
    public int cardsPerPage = 8;
    public Text pageText;
    [SerializeField] private int totalNumbers; // 用于显示总卡片数量
    private int maxPage; // 用于翻页时计算最大页数

    public Button nextPageButton;
    public Button previousPageButton;
    public Button closeButton;


    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        totalNumbers = AchievementManager.Instance.achievementList.achievement.Count;
        UpdateMaxPage();
        DisplayCards(page);
        UpdatePageUI();

        nextPageButton.onClick.AddListener(NextPage);
        previousPageButton.onClick.AddListener(PreviousPage);
        closeButton.onClick.AddListener(CloseButton);
    }

    public void CloseButton()
    {
        UIManager.Instance.ClosePanel(panelName);
    }

    private void UpdateMaxPage()
    {
        maxPage = Mathf.CeilToInt((float)totalNumbers / cardsPerPage) - 1;
    }

    private void UpdatePageUI()
    {
        pageText.text = $"{page + 1}/{maxPage + 1}";
    }

    private void DisplayCards(int _page)
    {
        ResetCardSlots();

        List<AchievementSO> cardsToDisplay = AchievementManager.Instance.achievementList.achievement;

        for (int i = 0; i < cardsToDisplay.Count; i++)
        {
            if (i >= _page * cardsPerPage && i < (_page + 1) * cardsPerPage)
            {
                AchievementSO achievement = cardsToDisplay[i];
                GameObject slot = cardSlots[i % cardsPerPage];

                slot.gameObject.SetActive(true);
                achievement.ApplyVisuals(slot, heldColor, notHeldColor);
                AnimateCard(slot, 4);
            }
        }
    }

    private void AnimateCard(GameObject cardSlot, int punchStrength)
    {
        cardSlot.transform.DOPunchRotation(new Vector3(0, 0, punchStrength), 0.2f, 4, 0.5f);
        cardSlot.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
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
        if (page < maxPage)
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