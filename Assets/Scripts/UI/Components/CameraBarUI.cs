using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraBarUI : MonoBehaviour
{
    [Header("UI 控件")]
    public Slider angleSlider; // 俯仰角度滑块

    private PerspectiveCameraController cameraController;
    
    private float[] snapValues = { 10f, 30f, 60f }; // 吸附档位
    private float snapThreshold = 5f; // 吸附阈值

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

        // 添加 UI 事件监听，控制相机的旋转状态
        EventTrigger trigger = angleSlider.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = angleSlider.gameObject.AddComponent<EventTrigger>();
        }

        // 当用户按下滑块时，设置相机处于旋转状态
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        trigger.triggers.Add(pointerDownEntry);

        // 当用户抬起滑块时，取消相机的旋转状态
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        trigger.triggers.Add(pointerUpEntry);
    }

    private void OnAngleChanged(float value)
    {
        if (cameraController != null)
        {
            // 计算与每个目标档位的距离，并找到最近的档位
            float closestValue = snapValues[0];
            float closestDistance = Mathf.Abs(value - closestValue);

            foreach (float snapValue in snapValues)
            {
                float distance = Mathf.Abs(value - snapValue);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestValue = snapValue;
                }
            }

            // 如果距离小于阈值，则吸附到该档位
            if (closestDistance < snapThreshold)
            {
                value = closestValue;
                angleSlider.value = value;
            }

            // 设置相机的俯仰角
            cameraController.angle_x = value;
        }
    }
}
