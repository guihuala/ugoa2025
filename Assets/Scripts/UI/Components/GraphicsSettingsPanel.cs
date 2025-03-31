using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsSettingsManager : MonoBehaviour
{
    [Header("UI组件")] 
    public Dropdown qualityPresetDropdown;  // 质量预设下拉菜单
    
    private bool isInitialized = false;

    private void Start()
    {
        LoadSettings();
        InitializeSettings();
    }

    private void InitializeSettings()
    {
        // 初始化画质预设下拉菜单
        qualityPresetDropdown.ClearOptions();

        // 自定义的质量档位映射，映射到中文名称
        List<string> qualityOptions = new List<string>
        {
            "低",    // 低画质
            "中",    // 中画质
            "高",    // 高画质
        };

        // 向下拉框添加自定义的中文质量选项
        qualityPresetDropdown.AddOptions(qualityOptions);

        // 设置画质下拉框的当前值为保存的画质
        qualityPresetDropdown.value = GetSavedQualityLevel();
        qualityPresetDropdown.onValueChanged.AddListener(OnQualityPresetChanged);

        isInitialized = true;
    }

    #region UI事件

    public void OnQualityPresetChanged(int qualityIndex)
    {
        if (!isInitialized) return;

        // 设置对应的画质
        QualitySettings.SetQualityLevel(qualityIndex);
        
        // 保存设置
        SaveSettings();
    }
    
    #endregion

    #region 保存预设

    public void SaveSettings()
    {
        // 保存当前质量等级到 PlayerPrefs
        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        // 加载保存的画质设置，默认值为当前画质
        int qualityLevel = GetSavedQualityLevel();
        QualitySettings.SetQualityLevel(qualityLevel);
    }

    // 获取保存的画质设置，如果没有保存则返回默认的画质
    private int GetSavedQualityLevel()
    {
        return PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
    }

    #endregion
}