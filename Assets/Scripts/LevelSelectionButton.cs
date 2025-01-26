using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private bool unlocked;
    [SerializeField] private Image unlockImage;
    [SerializeField] private SceneName levelName;

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener((() =>
        {
            PressSelection(levelName);
        }));
    }

    // 每个关卡单独读取
    private void Update()
    {
        UpdateLevelImage();
        UpdateLevelStatus();
    }

    private void UpdateLevelStatus()
    {
        // 读取存档
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