using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsPanel : BasePanel
{
    [Header("Volume Sliders")]
    public Slider mainVolumeSlider; // 控制全局音量的滑动条
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    [Header("Buttons")]
    public Button resumeButton;     // 恢复游戏按钮
    public Button exitButton;       // 退出游戏按钮

    protected override void Awake()
    {
        base.Awake();

        // 初始化滑动条的值
        mainVolumeSlider.value = AudioManager.Instance.mainVolume;
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // 添加滑动条事件监听
        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        // 按钮事件绑定
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // 调用父类的动画方法

        // 使用 DOTween 延迟设置 Time.timeScale
        DOTween.Sequence()
            .AppendInterval(0.5f)
            .AppendCallback(() =>
            {
                Time.timeScale = 0;
            });
    }

    public override void ClosePanel()
    {
        Time.timeScale = 1;
        
        base.ClosePanel();
    }

    /// <summary>
    /// 当全局音量滑动条改变时的回调
    /// </summary>
    /// <param name="value">滑动条当前值</param>
    private void OnMainVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeMainVolume(value);
    }

    /// <summary>
    /// 当BGM音量滑动条改变时的回调
    /// </summary>
    /// <param name="value">滑动条当前值</param>
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }

    /// <summary>
    /// 当SFX音量滑动条改变时的回调
    /// </summary>
    /// <param name="value">滑动条当前值</param>
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    /// <summary>
    /// 恢复游戏按钮点击回调
    /// </summary>
    private void OnResumeButtonClicked()
    {
        UIManager.Instance.ClosePanel("SettingPanel"); // 关闭面板
    }

    /// <summary>
    /// 退出游戏按钮点击回调
    /// </summary>
    private void OnExitButtonClicked()
    {
        // 如果需要退出到主菜单，可以在这里实现相关逻辑
        Debug.Log("Exiting game...");
    }
}
