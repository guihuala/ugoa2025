using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private bool unlocked;
    [SerializeField] private Image unlockImage;
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
    }

    // 每个关卡单独读取
    private void Update()
    {
        UpdateLevelImage();
        UpdateLevelStatus();
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
        if (!unlocked)
        {
            unlockImage.gameObject.SetActive(false);
        }
        else
        {
            unlockImage.gameObject.SetActive(true);
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