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
            });
    }
    
    private void Start()
    {
        replayBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
            
            SceneLoader.Instance.LoadScene(levelInfo.currentScene, "重新开始...");
            
            UIManager.Instance.RemovePanel(panelName);
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            SceneLoader.Instance.LoadScene(SceneName.Title,"回到主界面...");
            
            UIManager.Instance.RemovePanel(panelName);
        });
    }
}
