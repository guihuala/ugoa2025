using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class GameVictoryPanel : BasePanel
{
    [SerializeField] private Button nextLevelBtn;
    [SerializeField] private Button mainMenuBtn;
    
    [SerializeField] private Image victoryImg;
    
    [SerializeField] private Sprite[] victorySprites; // 存储不同关卡的胜利图片

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);
        
        // 获取当前关卡信息
        LevelInfo levelInfo = FindObjectOfType<LevelInfo>();
        if (levelInfo != null)
        {
            int index = 0;
            if(levelInfo.currentScene == SceneName.Level1)
                index = 0;
            else if (levelInfo.currentScene == SceneName.Level2)
                index = 1;
            else if (levelInfo.currentScene == SceneName.Level3)
                index = 2;
            
            SetVictoryImage(index); // 传入当前关卡索引
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
            
            SceneLoader.Instance.LoadScene(SceneName.Title, "回到主界面...");
        });
    }

    /// <summary>
    /// 根据关卡索引设置胜利界面的图片
    /// </summary>
    private void SetVictoryImage(int levelIndex)
    {
        if (victorySprites != null && victorySprites.Length > 0)
        {
            // 确保索引在有效范围内
            if (levelIndex >= 0 && levelIndex < victorySprites.Length)
            {
                victoryImg.sprite = victorySprites[levelIndex];
            }
            else
            {
                victoryImg.sprite = victorySprites[0]; // 默认显示第一张图片
            }
        }
        else
        {
            Debug.LogError("VictorySprites 数组为空，无法设置胜利图片");
        }
    }
}
