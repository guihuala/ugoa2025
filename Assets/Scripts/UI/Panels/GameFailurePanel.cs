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
        replayBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
            
            UIManager.Instance.ClosePanel(panelName);
            
            SceneLoader.Instance.LoadScene(levelInfo.currentScene, "重新开始...");
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            UIManager.Instance.ClosePanel(panelName);
            
            SceneLoader.Instance.LoadScene(SceneName.Title, "回到主界面...");
        });
    }
}
