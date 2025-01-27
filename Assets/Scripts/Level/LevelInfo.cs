using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    // 配置每一关的基本信息
    public SceneName currentScene;
    public SceneName nextLevel;
    public string nextLevelName;
    
    // 是否是最后一关
    private bool isEndLevel;

    public void GoToNextLevel()
    {
        // 在这边存储一下进入下一关
        if (!isEndLevel)
        {
            SceneLoader.Instance.LoadScene(nextLevel, "下一关...");
            LevelManager.Instance.UnlockLevel(nextLevelName);
            SaveManager.Instance.NewRecord();
        }
    }
}
