using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PictureFramePanel : BasePanel
{
    [Header("组件配置")]
    [SerializeField] private Image[] photos;
    [SerializeField] private Button closeBtn;
    
    private RectTransform panelRectTransform;

    protected override void Awake()
    {
        base.Awake();
        panelRectTransform = transform as RectTransform;
    }

    private void Start()
    {
        closeBtn.onClick.AddListener(ClosePanel);
        
        InitUI();
    }

    private void InitUI()
    {
        int index = 0;
        
        foreach (var photo in photos)
        {
            LevelData requiredLevel = LevelManager.Instance.levels[index];

            if (requiredLevel != null && requiredLevel.isUnlocked && requiredLevel.isPlayed)
            {
                photo.gameObject.SetActive(true);
                index++;
            }
            else
            {
                photo.gameObject.SetActive(false);
                return;
            }
        }
    }

    public override void OpenPanel(string name)
    {
        panelName = name;
        // 激活面板
        gameObject.SetActive(true);
        PanelSlideIn();
    }

    public override void ClosePanel()
    {
        hasRemoved = true;
        PanelSlideOut();
    }

    // 从上至下的进入动画
    private void PanelSlideIn()
    {
        if (panelRectTransform != null)
        {
            // 设置初始位置为屏幕外的顶部
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, Screen.height);
            // 动画播放到目标位置
            panelRectTransform.DOAnchorPosY(0f, 0.5f).SetEase(Ease.OutBack);
        }
    }

    // 从下至上的退出动画，并在结束后隐藏面板
    private void PanelSlideOut()
    {
        if (panelRectTransform != null)
        {
            // 设置初始位置为当前的屏幕内
            panelRectTransform.anchoredPosition = new Vector2(panelRectTransform.anchoredPosition.x, 0f);
            // 动画播放到屏幕外的底部
            panelRectTransform.DOAnchorPosY(Screen.height, 0.5f).SetEase(Ease.InBack).OnKill(() =>
            {
                // 动画播放完后隐藏面板
                gameObject.SetActive(false);
            });
        }
    }
}