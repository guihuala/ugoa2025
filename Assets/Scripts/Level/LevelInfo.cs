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
    public bool isEndLevel;

    public void GoToNextLevel()
    {
        // 在这边存储一下进入下一关
        // 把场景改为下一关或者选关场景
        if (!isEndLevel)
        {
            SceneLoader.Instance.LoadScene(nextLevel, "下一关...");
            LevelManager.Instance.UnlockLevel(nextLevelName);
            SaveManager.Instance.NewRecord();
        }
    }

    public void VictorySaveLevel()
    {
        LevelManager.Instance.UnlockLevel(nextLevelName);
        SaveManager.Instance.NewRecord();
    }
}
