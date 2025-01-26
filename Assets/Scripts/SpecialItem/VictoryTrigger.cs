using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour , IEnterSpecialItem
{
    [SerializeField] private bool isEndLevel;
    [SerializeField] private SceneName nextScene;
    
    public void Apply()
    {
        // 在这边存储一下进入下一关
        SceneLoader.Instance.LoadScene(nextScene,"...");
        SaveManager.Instance.NewRecord();
    }
}
