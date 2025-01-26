using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    [SerializeField] private bool isEndLevel;
    [SerializeField] private string nextLevelName;
    [SerializeField] private SceneName nextScene;
    
    public void Apply()
    {
        // 在这边存储一下进入下一关
        if (!isEndLevel)
        {
            SceneLoader.Instance.LoadScene(nextScene, "前往下一关...");
            LevelManager.Instance.UnlockLevel(nextLevelName);
            SaveManager.Instance.NewRecord();
        }
        else // 通关的情况
        {
            
        }
    }
}
