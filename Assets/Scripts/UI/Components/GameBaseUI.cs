using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameBaseUI : MonoBehaviour
{
    [SerializeField] private Button pauseBtn;
    [SerializeField] private Text stepText;

    [SerializeField] private Button testSaveBtn;

    private void Awake()
    {
        pauseBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPanel("SettingPanel");
        });
        
        testSaveBtn.onClick.AddListener(() =>
        {
            UIManager.Instance.OpenPanel("SavePanel");
        });
    }
    
}
