using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSetting : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    private void Awake()
    {
        // 初始化音量滑动条的默认值
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // 添加音量滑动条的监听事件
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);
    }
    
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }
    
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }
}
