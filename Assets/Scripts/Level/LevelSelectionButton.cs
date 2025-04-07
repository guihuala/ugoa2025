using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private bool unlocked;

    [SerializeField] private GameObject themeUnlockImg;
    [SerializeField] private GameObject themeLockImg;
    
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
            if (level.requiresItems)
            {
                LevelManager.Instance.UnlockSpecialLevel(level);
            }
            
            unlocked = level.isUnlocked; // 设置解锁状态
        }
    }

    private void UpdateLevelImage()
    {
        if (unlocked)
        {
            levelLockImg.SetActive(false);
        }
        else
        {
            levelLockImg.SetActive(true);
        }
        
        SetLockImg(unlocked);
        SetUnlockImg(unlocked);
    }

    void SetLockImg(bool isUnlocked)
    {
        if(themeUnlockImg != null)
            levelLockImg.SetActive(!isUnlocked);
    }

    void SetUnlockImg(bool isUnlocked)
    {
        if(themeUnlockImg != null)
            themeUnlockImg.SetActive(isUnlocked);
    }

    public void PressSelection(SceneName _LevelName)
    {
        if (unlocked)
        {
            SaveManager.Instance.playTime++;
            LevelManager.Instance.PlayLevel(levelName);
            SceneLoader.Instance.LoadScene(_LevelName,"...");
        }
    }
}