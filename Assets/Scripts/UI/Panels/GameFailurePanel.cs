using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameFailurePanel : BasePanel
{
    [SerializeField] private Button replayBtn;
    [SerializeField] private Button mainMenuBtn;
    [SerializeField] private Text titleText;

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
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
}
