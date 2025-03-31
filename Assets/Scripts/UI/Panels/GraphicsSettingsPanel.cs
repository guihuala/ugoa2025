using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class GraphicsSettingsManager : BasePanel
{
    [Header("UI组件")] public Dropdown qualityPresetDropdown;
    public Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public Slider shadowDistanceSlider;
    public Slider antiAliasingSlider;
    public Slider textureQualitySlider;
    public Toggle vSyncToggle;
    public Dropdown anisotropicFilteringDropdown;
    public Dropdown shadowResolutionDropdown;
    public Toggle softParticlesToggle;
    public Toggle dynamicResolutionToggle;

    private Resolution[] resolutions;
    private bool isInitialized = false;

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name);

        InitializeSettings();
    }

    private void InitializeSettings()
    {
        // 初始化画质预设下拉菜单
        qualityPresetDropdown.ClearOptions();
        qualityPresetDropdown.AddOptions(new List<string>(QualitySettings.names));
        qualityPresetDropdown.onValueChanged.AddListener(OnQualityPresetChanged);

        // 初始化分辨率设置
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @ " + resolutions[i].refreshRate +
                            "Hz";
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        // 初始化其他UI元素
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        shadowDistanceSlider.onValueChanged.AddListener(OnShadowDistanceChanged);
        antiAliasingSlider.onValueChanged.AddListener(OnAntiAliasingChanged);
        textureQualitySlider.onValueChanged.AddListener(OnTextureQualityChanged);
        vSyncToggle.onValueChanged.AddListener(OnVSyncChanged);
        anisotropicFilteringDropdown.onValueChanged.AddListener(OnAnisotropicFilteringChanged);
        shadowResolutionDropdown.onValueChanged.AddListener(OnShadowResolutionChanged);
        softParticlesToggle.onValueChanged.AddListener(OnSoftParticlesChanged);
        dynamicResolutionToggle.onValueChanged.AddListener(OnDynamicResolutionChanged);

        // 加载保存的设置
        LoadSettings();

        isInitialized = true;
    }

    #region UI事件

    public void OnQualityPresetChanged(int qualityIndex)
    {
        if (!isInitialized) return;

        QualitySettings.SetQualityLevel(qualityIndex);
        UpdateUIFromQualitySettings();
        SaveSettings();
    }

    public void OnResolutionChanged(int resolutionIndex)
    {
        if (!isInitialized) return;

        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen, resolution.refreshRate);
        SaveSettings();
    }

    public void OnFullscreenChanged(bool isFullscreen)
    {
        if (!isInitialized) return;

        Screen.fullScreen = isFullscreen;
        SaveSettings();
    }

    public void OnShadowDistanceChanged(float distance)
    {
        if (!isInitialized) return;

        QualitySettings.shadowDistance = distance;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnAntiAliasingChanged(float level)
    {
        if (!isInitialized) return;

        QualitySettings.antiAliasing = (int)level;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnTextureQualityChanged(float level)
    {
        if (!isInitialized) return;

        QualitySettings.globalTextureMipmapLimit = (int)level;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnVSyncChanged(bool enabled)
    {
        if (!isInitialized) return;

        QualitySettings.vSyncCount = enabled ? 1 : 0;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnAnisotropicFilteringChanged(int mode)
    {
        if (!isInitialized) return;

        QualitySettings.anisotropicFiltering = (AnisotropicFiltering)mode;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnShadowResolutionChanged(int level)
    {
        if (!isInitialized) return;

        QualitySettings.shadowResolution = (ShadowResolution)level;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnSoftParticlesChanged(bool enabled)
    {
        if (!isInitialized) return;

        QualitySettings.softParticles = enabled;
        SetCustomQualityLevel();
        SaveSettings();
    }

    public void OnDynamicResolutionChanged(bool enabled)
    {
        if (!isInitialized) return;

        if (enabled)
        {
            DynamicResolutionHandler.SetDynamicResScaler(DynamicResScaler, DynamicResScalePolicyType.ReturnsPercentage);
        }
        else
        {
            DynamicResolutionHandler.SetDynamicResScaler(null, DynamicResScalePolicyType.ReturnsPercentage);
        }

        SaveSettings();
    }

    #endregion

    #region 具体修改方法

    private void SetCustomQualityLevel()
    {
        // 当任何自定义设置被修改时，切换到"自定义"预设
        if (qualityPresetDropdown.value != QualitySettings.names.Length - 1)
        {
            qualityPresetDropdown.value = QualitySettings.names.Length - 1;
        }
    }

    private float DynamicResScaler()
    {
        // 简单的动态分辨率缩放器
        float targetFrameRate = 60f;
        float frameTime = Time.unscaledDeltaTime;
        float targetFrameTime = 1f / targetFrameRate;

        // 根据帧时间调整分辨率比例
        float scale = Mathf.Clamp(targetFrameTime / frameTime, 0.5f, 1.0f);
        return scale;
    }

    private void UpdateUIFromQualitySettings()
    {
        // 根据当前质量设置更新UI元素
        shadowDistanceSlider.value = QualitySettings.shadowDistance;
        antiAliasingSlider.value = QualitySettings.antiAliasing;
        textureQualitySlider.value = QualitySettings.globalTextureMipmapLimit;
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        anisotropicFilteringDropdown.value = (int)QualitySettings.anisotropicFiltering;
        shadowResolutionDropdown.value = (int)QualitySettings.shadowResolution;
        softParticlesToggle.isOn = QualitySettings.softParticles;
    }

    #endregion

    #region 保存预设

    public void SaveSettings()
    {
        PlayerPrefs.SetInt("QualityLevel", QualitySettings.GetQualityLevel());
        PlayerPrefs.SetInt("ResolutionIndex", resolutionDropdown.value);
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        PlayerPrefs.SetFloat("ShadowDistance", QualitySettings.shadowDistance);
        PlayerPrefs.SetInt("AntiAliasing", QualitySettings.antiAliasing);
        PlayerPrefs.SetInt("TextureQuality", QualitySettings.globalTextureMipmapLimit);
        PlayerPrefs.SetInt("VSync", QualitySettings.vSyncCount);
        PlayerPrefs.SetInt("AnisotropicFiltering", (int)QualitySettings.anisotropicFiltering);
        PlayerPrefs.SetInt("ShadowResolution", (int)QualitySettings.shadowResolution);
        PlayerPrefs.SetInt("SoftParticles", QualitySettings.softParticles ? 1 : 0);
        PlayerPrefs.SetInt("DynamicResolution", dynamicResolutionToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void LoadSettings()
    {
        // 加载质量预设
        int qualityLevel = PlayerPrefs.GetInt("QualityLevel", QualitySettings.GetQualityLevel());
        QualitySettings.SetQualityLevel(qualityLevel);
        qualityPresetDropdown.value = qualityLevel;

        // 加载分辨率
        int resolutionIndex = PlayerPrefs.GetInt("ResolutionIndex", GetCurrentResolutionIndex());
        resolutionIndex = Mathf.Clamp(resolutionIndex, 0, resolutions.Length - 1);
        resolutionDropdown.value = resolutionIndex;
        OnResolutionChanged(resolutionIndex);

        // 加载其他设置
        fullscreenToggle.isOn = PlayerPrefs.GetInt("Fullscreen", Screen.fullScreen ? 1 : 0) == 1;
        QualitySettings.shadowDistance = PlayerPrefs.GetFloat("ShadowDistance", QualitySettings.shadowDistance);
        QualitySettings.antiAliasing = PlayerPrefs.GetInt("AntiAliasing", QualitySettings.antiAliasing);
        QualitySettings.globalTextureMipmapLimit =
            PlayerPrefs.GetInt("TextureQuality", QualitySettings.globalTextureMipmapLimit);
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSync", QualitySettings.vSyncCount);
        QualitySettings.anisotropicFiltering =
            (AnisotropicFiltering)PlayerPrefs.GetInt("AnisotropicFiltering", (int)QualitySettings.anisotropicFiltering);
        QualitySettings.shadowResolution =
            (ShadowResolution)PlayerPrefs.GetInt("ShadowResolution", (int)QualitySettings.shadowResolution);
        QualitySettings.softParticles = PlayerPrefs.GetInt("SoftParticles", QualitySettings.softParticles ? 1 : 0) == 1;
        dynamicResolutionToggle.isOn = PlayerPrefs.GetInt("DynamicResolution", 0) == 1;

        // 更新UI显示
        UpdateUIFromQualitySettings();

        // 应用动态分辨率
        OnDynamicResolutionChanged(dynamicResolutionToggle.isOn);
    }

    private int GetCurrentResolutionIndex()
    {
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == Screen.currentResolution.width &&
                resolutions[i].height == Screen.currentResolution.height &&
                resolutions[i].refreshRate == Screen.currentResolution.refreshRate)
            {
                return i;
            }
        }

        return 0;
    }

    #endregion
}