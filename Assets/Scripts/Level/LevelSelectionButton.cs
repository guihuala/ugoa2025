using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private bool unlocked;

    [SerializeField] private GameObject levelUnlockImg;
    [SerializeField] private GameObject levelLockImg;
    
    [SerializeField] private string levelName;
    [SerializeField] private SceneName sceneName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener((() =>
        {
            PressSelection(sceneName);
        }));
    }

    private void Start()
    {
        UpdateLevelStatus();
        
        UpdateLevelImage();
    }

    private void UpdateLevelStatus()
    {
        var level = LevelManager.Instance.levels.Find(l => l.name == levelName);
        if (level != null)
        {
            unlocked = level.isUnlocked; // 设置解锁状态
        }
    }

    private void UpdateLevelImage()
    {
        if(levelName == "Level1")
            return;
        
        if (!unlocked)
        {
            levelLockImg.SetActive(true);
            levelUnlockImg.SetActive(false);
            
            gameObject.SetActive(false);
        }
        else
        {
            levelLockImg.SetActive(false);
            levelUnlockImg.SetActive(true);
            
            gameObject.SetActive(true);
        }
    }

    public void PressSelection(SceneName _LevelName)
    {
        if (unlocked)
        {
            SceneLoader.Instance.LoadScene(_LevelName,"...");
        }
    }
}