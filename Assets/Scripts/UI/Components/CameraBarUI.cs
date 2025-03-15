using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraBarUI : MonoBehaviour
{
    [Header("UI 控件")]
    public Slider angleSlider; // 俯仰角度滑块

    private PerspectiveCameraController cameraController;

    private void Start()
    {
        // 找到场景中的 PerspectiveCameraController 组件
        cameraController = FindObjectOfType<PerspectiveCameraController>();

        if (cameraController == null)
        {
            Debug.LogError("CameraController 未找到，请确保场景中有 PerspectiveCameraController");
            return;
        }
        
        angleSlider.minValue = 10f;
        angleSlider.maxValue = 60f;
        angleSlider.value = cameraController.angle_x;
        angleSlider.onValueChanged.AddListener(OnAngleChanged);
    }

    private void OnAngleChanged(float value)
    {
        if (cameraController != null)
        {
            cameraController.angle_x = value;
        }
    }
}
