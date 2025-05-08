using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// 在这里配置游戏的场景枚举，名称需要与场景名一致
public enum SceneName
{
    Title,
    LevelSelection,
    
    Level1_1,
    Level1_2,
    Level1_3,
    Level1_4,
    
    Level1_3_story,
    
    Level2_1,
    Level2_2,
    Level2_3,
    Level2_4,
    
    Level3_1,
    Level3_2,
    Level3_3,
    Level3_4,
    
    TutorialScene,
    OfficeScene,
    
    CG,
}

public class SceneLoader : SingletonPersistent<SceneLoader>
{
    private float fadeDuration = 1.5f;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        UIManager.Instance.OpenPanel("SceneLoadedBlackPanel");
        UIManager.Instance.ClosePanel("SceneLoadedBlackPanel");
    }

    public void LoadScene(SceneName sceneName, string loadStr)
    {
        SleepBlackPanel sleepBlackPanel = UIManager.Instance.OpenPanel("SleepBlackPanel") as SleepBlackPanel;

        if (!sleepBlackPanel) return;

        sleepBlackPanel.StartSleepCounting(fadeDuration, loadStr, () =>
        {
            // 保存当前场景名到 PlayerPrefs
            PlayerPrefs.SetString("LastSceneName", SceneManager.GetActiveScene().name);
            PlayerPrefs.Save(); // 确保立即保存
            
            // 使用枚举值的字符串表示加载场景
            SceneManager.LoadScene(sceneName.ToString());
            UIManager.Instance.RemovePanel("SleepBlackPanel");
            
            // 改变一下存档管理器当前的场景
            SaveManager.Instance.scensName = sceneName;
        },sceneName);

        AchievementManager.Instance.ClearPendingAchievements();
    }
    
    /// <summary>
    /// 普通黑屏过场
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(SceneName sceneName)
    {
        BlackPanel blackPanel = UIManager.Instance.OpenPanel("BlackPanel") as BlackPanel;

        if (!blackPanel) return;

        blackPanel.StartCounting(fadeDuration, () =>
        {
            SceneManager.LoadScene(sceneName.ToString());
            UIManager.Instance.RemovePanel("BlackPanel");
        });
    }
}