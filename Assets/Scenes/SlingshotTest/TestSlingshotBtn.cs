using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSlingshotBtn : MonoBehaviour
{
    private Button btn;
    private Text btnText;
    private bool isUsing = false;

    private void Start()
    {
        btn = GetComponent<Button>();
        btnText = btn.GetComponentInChildren<Text>();
        
        btn.onClick.AddListener(ToggleBtn);
        
        UpdateButtonText();
    }

    void ToggleBtn()
    {
        isUsing = !isUsing;
        
        FindObjectOfType<PerspectiveCameraController>().allowCameraControl = !FindObjectOfType<PerspectiveCameraController>().allowCameraControl;
        FindObjectOfType<ClickableEffect>().isActive = !FindObjectOfType<ClickableEffect>().isActive;
        
        UpdateButtonText();
    }
    
    void UpdateButtonText()
    {
        if (isUsing)
        {
            btnText.text = "停用\n弹弓";
        }
        else
        {
            btnText.text = "使用\n弹弓";
        }
    }
}