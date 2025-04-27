using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelInfo : MonoBehaviour
{
    // 配置每一关的基本信息
    public SceneName currentScene;
    [Tooltip("用于场景传送")]public SceneName nextLevel;
    [Tooltip("用于解锁")]public string nextLevelName;

    public DialogueData levelDialogue;
    
    // 是否是最后一关
    public bool isEndLevel;

    private void Start()
    {
        if (levelDialogue != null)
        {
            StartCoroutine(StartDialogueWithDelay());
        }
    }

    private IEnumerator StartDialogueWithDelay()
    {
        yield return new WaitForSeconds(1f);
        DialoguePanel dialoguePanel = UIManager.Instance.OpenPanel("DialoguePanel") as DialoguePanel;
        dialoguePanel.StartDialogue(levelDialogue);
    }

    public void GoToNextLevel()
    {
        // 在这边存储一下进入下一关
        // 把场景改为下一关或者选关场景
        if (!isEndLevel)
        {
            SaveManager.Instance.playTime++;
            
            SceneLoader.Instance.LoadScene(nextLevel, "下一关...");
            LevelManager.Instance.UnlockLevel(nextLevelName);
        }
    }

    public void VictorySaveLevel()
    {
        if (isEndLevel)
        {
            SaveManager.Instance.isComplete = true;
        }
        else
        {
            LevelManager.Instance.UnlockLevel(nextLevelName);
        }
    }
}