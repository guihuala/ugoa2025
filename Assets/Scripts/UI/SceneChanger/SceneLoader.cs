using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;

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
    private float fadeDuration = 1f;
    private float minLoadTime = 1f; // 最小加载时间

    private bool isLoading = false;
    private AsyncOperation currentLoadingOperation;

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

    public async void LoadScene(SceneName sceneName, string loadStr)
    {
        if (isLoading) return;
        isLoading = true;

        SleepBlackPanel sleepBlackPanel = UIManager.Instance.OpenPanel("SleepBlackPanel",true) as SleepBlackPanel;
        if (!sleepBlackPanel)
        {
            isLoading = false;
            return;
        }
        
        sleepBlackPanel.StartSleepCounting(fadeDuration, loadStr, null, sceneName);
        
        await Task.Delay((int)(fadeDuration * 1000));
        
        PlayerPrefs.SetString("LastSceneName", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
        
        currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName.ToString());
        currentLoadingOperation.allowSceneActivation = false;

        float loadStartTime = Time.time;
        float progress = 0;
        
        while (!currentLoadingOperation.isDone)
        {
            if (currentLoadingOperation.progress >= 0.9f)
            {
                // 等待最小加载时间结束
                if (Time.time - loadStartTime >= minLoadTime)
                {
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }

            await Task.Yield();
        }

        // 更新保存管理器
        SaveManager.Instance.scensName = sceneName;
        
        UIManager.Instance.ClosePanel("SleepBlackPanel");
        currentLoadingOperation = null;
        isLoading = false;

        AchievementManager.Instance.ClearPendingAchievements();
    }

    public async void LoadScene(SceneName sceneName)
    {
        if (isLoading) return;
        isLoading = true;

        BlackPanel blackPanel = UIManager.Instance.OpenPanel("BlackPanel",true) as BlackPanel;
        if (!blackPanel)
        {
            isLoading = false;
            return;
        }

        // 淡出
        blackPanel.StartCounting(fadeDuration, null);
        
        await Task.Delay((int)(fadeDuration * 1000));
        
        currentLoadingOperation = SceneManager.LoadSceneAsync(sceneName.ToString());
        currentLoadingOperation.allowSceneActivation = false;

        float loadStartTime = Time.time;
        
        while (!currentLoadingOperation.isDone)
        {
            if (currentLoadingOperation.progress >= 0.9f)
            {
                if (Time.time - loadStartTime >= minLoadTime)
                {
                    currentLoadingOperation.allowSceneActivation = true;
                }
            }

            await Task.Yield();
        }
        
        UIManager.Instance.ClosePanel("BlackPanel");
        currentLoadingOperation = null;
        isLoading = false;
    }
}