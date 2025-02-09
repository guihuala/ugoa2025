using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PauseSettingsPanel : BasePanel
{
    [Header("Volume Sliders")]
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    [Header("Buttons")]
    [SerializeField] private Button bgmVolumeButton; // 显示/隐藏BGM音量滑动条按钮
    [SerializeField] private Button sfxVolumeButton; // 显示/隐藏SFX音量滑动条按钮
    [SerializeField] private Button resumeButton; // 恢复游戏按钮
    [SerializeField] private Button menuButton; // 返回菜单按钮
    [SerializeField] private Button exitButton; // 退出游戏按钮

    protected override void Awake()
    {
        base.Awake();

        // 初始化音量滑动条的默认值
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // 添加音量滑动条的监听事件
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        // 按钮事件
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        menuButton.onClick.AddListener(OnMenuButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);

        // 音量按钮事件
        bgmVolumeButton.onClick.AddListener(ToggleBgmVolumeSlider);
        sfxVolumeButton.onClick.AddListener(ToggleSfxVolumeSlider);
    }

    private void Start()
    {
        bgmVolumeSlider.gameObject.SetActive(false);
        sfxVolumeSlider.gameObject.SetActive(false);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // 调用基类的打开面板方法
        
        DOTween.Sequence()
            .AppendInterval(0.3f)
            .AppendCallback(() =>
            {
                Time.timeScale = 0; // 暂停游戏
            }).SetUpdate(true);
    }

    public override void ClosePanel()
    {
        Time.timeScale = 1; // 恢复游戏速度
        
        base.ClosePanel();
    }
    
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }
    
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    private void OnResumeButtonClicked()
    {
        UIManager.Instance.ClosePanel("SettingPanel"); // 关闭设置面板
    }

    private void OnMenuButtonClicked()
    {
        ConfirmationPanel confirmationPanel = UIManager.Instance.OpenPanel("ConfirmationPanel") as ConfirmationPanel;

        confirmationPanel.ShowConfirmation("确定要退出游戏吗？关卡内的进度将不会保存", () =>
        {
            Time.timeScale = 1;
            SceneLoader.Instance.LoadScene(SceneName.Title, "返回主界面...");

            // 防止面板还在字典中
            UIManager.Instance.RemovePanel("ConfirmationPanel");
            UIManager.Instance.RemovePanel("SettingPanel");
        });
    }

    private void OnExitButtonClicked()
    {
        Application.Quit();
    }

    // 显示/隐藏BGM音量滑动条
    private void ToggleBgmVolumeSlider()
    {
        bgmVolumeSlider.gameObject.SetActive(!bgmVolumeSlider.gameObject.activeSelf);
    }

    // 显示/隐藏SFX音量滑动条
    private void ToggleSfxVolumeSlider()
    {
        sfxVolumeSlider.gameObject.SetActive(!sfxVolumeSlider.gameObject.activeSelf);
    }
}
