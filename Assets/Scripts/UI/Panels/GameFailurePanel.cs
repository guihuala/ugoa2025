using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameFailurePanel : BasePanel
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Text titleText;
    
    [SerializeField] private Image FailureImg;
    [SerializeField] private Sprite[] FailureSprites;

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        // 获取当前关卡信息
        LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
        if (levelInfo != null)
        {
            int index = 0;
            if (levelInfo.currentScene == SceneName.Level1_1 || levelInfo.currentScene == SceneName.Level1_2 ||
                levelInfo.currentScene == SceneName.Level1_3 || levelInfo.currentScene == SceneName.Level1_4)
                index = 0;
            
            else if (levelInfo.currentScene == SceneName.Level2_1 || levelInfo.currentScene == SceneName.Level2_2 ||
                     levelInfo.currentScene == SceneName.Level2_3 || levelInfo.currentScene == SceneName.Level2_4)
                index = 1;
            
            else if (levelInfo.currentScene == SceneName.Level3_1 || levelInfo.currentScene == SceneName.Level3_2 ||
                     levelInfo.currentScene == SceneName.Level3_3 || levelInfo.currentScene == SceneName.Level3_4)
                index = 2;
            
            SetFailureImage(index); // 传入当前关卡索引
        }
        
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Time.timeScale = 0;
            }).SetUpdate(true);
    }
    
    private void Start()
    {
        SaveManager.Instance.failureTime++;
        
        titleText.text = LevelManager.Instance.failureReason;
        
        replayBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;

            // 增加一次游戏次数
            SaveManager.Instance.playTime++;
            
            // 从当前关卡重新开始
            LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
            UIManager.Instance.ClosePanel(panelName);
            SceneLoader.Instance.LoadScene(levelInfo.currentScene, "重新开始...");
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            UIManager.Instance.ClosePanel(panelName);
            
            SceneLoader.Instance.LoadScene(SceneName.LevelSelection, "...");
        });
    }
    
    /// <summary>
    /// 根据关卡索引设置胜利界面的图片
    /// </summary>
    private void SetFailureImage(int levelIndex)
    {
        if (FailureSprites != null && FailureSprites.Length > 0)
        {
            // 确保索引在有效范围内
            if (levelIndex >= 0 && levelIndex < FailureSprites.Length)
            {
                FailureImg.sprite = FailureSprites[levelIndex];
            }
            else
            {
                FailureImg.sprite = FailureSprites[0]; // 默认显示第一张图片
            }
        }
        else
        {
            Debug.LogError("VictorySprites 数组为空，无法设置胜利图片");
        }
    }
}
