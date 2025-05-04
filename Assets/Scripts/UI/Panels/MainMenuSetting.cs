using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuSetting : MonoBehaviour
{
    [Header("Volume Sliders")]
    public Slider bgmVolumeSlider;  // 控制背景音乐音量的滑动条
    public Slider sfxVolumeSlider;  // 控制音效音量的滑动条

    public Button closeBtn;
    public Button clearDataBtn; // 新增清除数据按钮

    [Header("Confirmation Panel")]
    public string confirmationPanelName = "ConfirmationPanel"; // 确认面板的名称
    public string clearAllMessage = "确定要清除所有游戏数据吗？此操作不可恢复！";

    private TitleUI titleUI;

    private void Awake()
    {
        titleUI = FindObjectOfType<TitleUI>();
        
        // 初始化音量滑动条的默认值
        bgmVolumeSlider.value = AudioManager.Instance.bgmVolumeFactor;
        sfxVolumeSlider.value = AudioManager.Instance.sfxVolumeFactor;

        // 添加音量滑动条的监听事件
        bgmVolumeSlider.onValueChanged.AddListener(OnBgmVolumeChanged);
        sfxVolumeSlider.onValueChanged.AddListener(OnSfxVolumeChanged);

        if (closeBtn != null)
            closeBtn.onClick.AddListener(() => gameObject.SetActive(false));

        // 初始化清除数据按钮
        if (clearDataBtn != null)
            clearDataBtn.onClick.AddListener(() => ShowClearDataConfirmation());
    }
    
    private void OnBgmVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeBgmVolume(value);
    }
    
    private void OnSfxVolumeChanged(float value)
    {
        AudioManager.Instance.ChangeSfxVolume(value);
    }

    /// <summary>
    /// 显示清除数据确认面板
    /// </summary>
    private void ShowClearDataConfirmation()
    {
        ConfirmationPanel panel = UIManager.Instance.OpenPanel(confirmationPanelName) as ConfirmationPanel;
        
        if (panel != null)
        {
            panel.ShowConfirmation(clearAllMessage, () => {
                ClearAllGameData();
                UIManager.Instance.ClosePanel(confirmationPanelName);
            });
        }
    }

    /// <summary>
    /// 清除所有游戏数据
    /// </summary>
    private void ClearAllGameData()
    {
        // 调用SAVE类的方法清除数据
        SAVE.DeleteAll();
        
        // 重新加载TitleUI状态
        if (titleUI != null)
        {
            // 禁用继续游戏和加载游戏按钮
            titleUI.Continue.interactable = false;
            titleUI.Load.interactable = false;
            
            // 关闭可能打开的面板
            if (titleUI.recordPanel != null) 
                titleUI.recordPanel.SetActive(false);
            if (titleUI.setPanel != null)
                titleUI.setPanel.SetActive(false);
            
            // 标记为首次游玩状态
            titleUI.isFirstTimePlay = true;
            
            // 重置RecordData的lastID
            RecordData.Instance.lastID = 123;
            RecordData.Instance.Save();
        }
    }
}