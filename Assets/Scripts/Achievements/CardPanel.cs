using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CardPanel : BasePanel
{
    #region STEP 01

    public GameObject[] cardSlots;

    [Header("Card Background Color")] public Color humanColor;
    public Color orcColor;
    public Color undeadColor;
    public Color heldColor;
    public Color notHeldColor;

    #endregion

    #region STEP 02

    public int page = 0;
    public Text pageText;

    #endregion

    #region STEP 04

    [SerializeField] private int totalNumbers; // 用于显示总卡片数量
    private int maxPage; // 用于翻页时计算最大页数

    #endregion

    #region STEP 05

    private bool isSearchByClass;
    private CardClass? currentSearchClass;

    #endregion

    #region STEP 06

    public Image cardBoardImage;
    [Header("Card Board Color")] public Color normalBgColor;
    public Color humanBgColor;
    public Color orcBgColor;
    public Color undeadBgColor;

    #endregion

    public Button nextPageButton;
    public Button previousPageButton;
    public Button closeButton; // 新增关闭按钮

    [Header("Page Settings")] public int cardsPerPage = 8;

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        cardBoardImage.color = normalBgColor;
        totalNumbers = CardManager.Instance.cards.Count;
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

        List<CardSO> cardsToDisplay = isSearchByClass && currentSearchClass.HasValue
            ? CardManager.Instance.GetCardsByClass(currentSearchClass.Value)
            : CardManager.Instance.cards;

        for (int i = 0; i < cardsToDisplay.Count; i++)
        {
            if (i >= _page * cardsPerPage && i < (_page + 1) * cardsPerPage)
            {
                CardSO card = cardsToDisplay[i];
                GameObject slot = cardSlots[i % cardsPerPage];

                slot.gameObject.SetActive(true);
                card.ApplyVisuals(slot, GetCardColor(card.cardClass), heldColor, notHeldColor);
                AnimateCard(slot, 4);
            }
        }
    }

    private void AnimateCard(GameObject cardSlot, int punchStrength)
    {
        cardSlot.transform.DOPunchRotation(new Vector3(0, 0, punchStrength), 0.2f, 4, 0.5f);
        cardSlot.transform.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.2f, 4, 0.5f);
    }

    public void SearchByClass(CardClass cardClass)
    {
        isSearchByClass = true;
        currentSearchClass = cardClass;

        List<CardSO> filteredCards = CardManager.Instance.GetCardsByClass(cardClass);
        totalNumbers = filteredCards.Count;
        page = 0;
        UpdateMaxPage();

        cardBoardImage.color = GetBackgroundColor(cardClass);
        DisplayCards(page);
        UpdatePageUI();
    }

    private Color GetBackgroundColor(CardClass cardClass)
    {
        switch (cardClass)
        {
            case CardClass.Human:
                return humanBgColor;
            case CardClass.Orc:
                return orcBgColor;
            case CardClass.Undead:
                return undeadBgColor;
            default:
                return normalBgColor;
        }
    }

    private void ResetCardSlots()
    {
        foreach (var slot in cardSlots)
        {
            slot.gameObject.SetActive(false);
        }
    }

    private Color GetCardColor(CardClass cardClass)
    {
        switch (cardClass)
        {
            case CardClass.Human:
                return humanColor;
            case CardClass.Orc:
                return orcColor;
            case CardClass.Undead:
                return undeadColor;
            default:
                return Color.white;
        }
    }

    public void ResetSearch()
    {
        isSearchByClass = false;
        totalNumbers = CardManager.Instance.cards.Count;
        page = 0;
        UpdateMaxPage();

        cardBoardImage.color = normalBgColor;
        DisplayCards(page);
        UpdatePageUI();
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