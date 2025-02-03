using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameVictoryPanel : BasePanel
{
    [SerializeField] private Button nextLevelBtn;
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
        nextLevelBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
            
            UIManager.Instance.ClosePanel(panelName);
            
            levelInfo.GoToNextLevel();
        });
        mainMenuBtn.onClick.AddListener(() =>
        {
            Time.timeScale = 1.0f;
            
            UIManager.Instance.ClosePanel(panelName);

            LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
            SaveManager.Instance.SetDefaultCurrentScene();
            levelInfo.VictorySaveLevel();
            
            SceneLoader.Instance.LoadScene(SceneName.Title,"回到主界面...");
        });
    }
}
