using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PauseSettingsPanel : BasePanel
{
    [Header("Volume Sliders")]
    public Slider mainVolumeSlider; // ����ȫ�������Ļ�����
    public Slider bgmVolumeSlider;  // ���Ʊ������������Ļ�����
    public Slider sfxVolumeSlider;  // ������Ч�����Ļ�����

    [Header("Buttons")]
    public Button resumeButton;     // �ָ���Ϸ��ť
    public Button exitButton;       // �˳���Ϸ��ť

    protected override void Awake()
    {
        base.Awake();

        // ��ʼ����������ֵ
        mainVolumeSlider.value = AudioManager.Instance.mainVolume;
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // ��ӻ������¼�����
        mainVolumeSlider.onValueChanged.AddListener(OnMainVolumeChanged);
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        // ��ť�¼���
        resumeButton.onClick.AddListener(OnResumeButtonClicked);
        exitButton.onClick.AddListener(OnExitButtonClicked);
    }

    public override void OpenPanel(string name)
    {
        base.OpenPanel(name); // ���ø���Ķ�������

        // ʹ�� DOTween �ӳ����� Time.timeScale
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
    /// ��ȫ�������������ı�ʱ�Ļص�
    /// </summary>
    /// <param name="value">��������ǰֵ</param>
    private void OnMainVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeMainVolume(value);
    }

    /// <summary>
    /// ��BGM�����������ı�ʱ�Ļص�
    /// </summary>
    /// <param name="value">��������ǰֵ</param>
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }

    /// <summary>
    /// ��SFX�����������ı�ʱ�Ļص�
    /// </summary>
    /// <param name="value">��������ǰֵ</param>
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    /// <summary>
    /// �ָ���Ϸ��ť����ص�
    /// </summary>
    private void OnResumeButtonClicked()
    {
        UIManager.Instance.ClosePanel("SettingPanel"); // �ر����
    }

    /// <summary>
    /// �˳���Ϸ��ť����ص�
    /// </summary>
    private void OnExitButtonClicked()
    {
        // �����Ҫ�˳������˵�������������ʵ������߼�
        Debug.Log("Exiting game...");
    }
}
