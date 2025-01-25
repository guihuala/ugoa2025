using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public enum TabType
{
    ClassTab,
    DefaultTab
}

public class TabsManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public TabType tabType;
    public CardClass associatedClass; // 关联的卡牌分类
    public CardPanel cardPanel;

    private Vector3 originalScale; // 保存原始缩放

    private void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (tabType == TabType.ClassTab)
        {
            transform.DOPunchRotation(new Vector3(0, 0, 30), 0.2f, 4, 0.5f);

            // 调用 UIManager 的分类筛选功能
            if (cardPanel != null)
            {
                cardPanel.SearchByClass(associatedClass);
            }
        }
        else if (tabType == TabType.DefaultTab)
        {
            transform.DOPunchRotation(new Vector3(0, 0, -30), 0.2f, 4, 0.5f);

            // 调用 UIManager 的重置搜索功能
            if (cardPanel != null)
            {
                cardPanel.ResetSearch();
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tabType == TabType.ClassTab || tabType == TabType.DefaultTab)
        {
            //LeanTween.scale(gameObject, new Vector3(1.5f, 1.5f, 1.5f), 0.5f).setEasePunch();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tabType == TabType.ClassTab || tabType == TabType.DefaultTab)
        {
            //LeanTween.scale(gameObject, originalScale, 0.3f).setEaseOutBack();
        }
    }
}