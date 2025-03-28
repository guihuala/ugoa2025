using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CameraBarUI : MonoBehaviour
{
    [Header("UI 控件")]
    public Slider angleSlider; // 俯仰角度滑块
    public CanvasGroup uiCanvasGroup;

    [Header("显示设置")]
    [SerializeField] private float showDuration = 3f; // 无操作后隐藏的延迟时间
    [SerializeField] private float fadeTime = 0.5f; // 淡入淡出时间

    private PerspectiveCameraController cameraController;
    private float[] snapValues = { 10f, 30f, 60f };
    private float snapThreshold = 5f;
    private float lastInteractionTime;
    private bool isUIVisible = true;

    private void Start()
    {
        cameraController = FindObjectOfType<PerspectiveCameraController>();
        if (cameraController == null)
        {
            Debug.LogError("CameraController 未找到，请确保场景中有 PerspectiveCameraController");
            return;
        }

        // 初始化UI
        if (uiCanvasGroup == null)
            uiCanvasGroup = GetComponent<CanvasGroup>();
        
        angleSlider.minValue = 10f;
        angleSlider.maxValue = 60f;
        angleSlider.value = cameraController.angle_x;
        angleSlider.onValueChanged.AddListener(OnAngleChanged);

        // 添加UI事件监听
        AddUIEventListeners();
        
        // 初始显示UI
        SetUIVisibility(true);
        lastInteractionTime = Time.time;
    }

    private void Update()
    {
        // 检测UI区域内的鼠标/触碰
        if (IsPointerOverUI())
        {
            lastInteractionTime = Time.time;
            if (!isUIVisible)
                SetUIVisibility(true);
        }
        // 超时隐藏
        else if (isUIVisible && Time.time - lastInteractionTime > showDuration)
        {
            SetUIVisibility(false);
        }
    }

    /// <summary>
    /// 检测鼠标/触摸是否在UI上
    /// </summary>
    private bool IsPointerOverUI()
    {
        // 鼠标检测（PC）
        if (EventSystem.current.IsPointerOverGameObject())
            return true;
        
        // 触摸检测（移动端）
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                return true;
        }
        
        return false;
    }

    /// <summary>
    /// 设置UI显示/隐藏（带淡入淡出效果）
    /// </summary>
    private void SetUIVisibility(bool visible)
    {
        isUIVisible = visible;
        StopAllCoroutines();
        FadeUI(visible ? 1f : 0f);
    }

    private void FadeUI(float targetAlpha)
    {
        uiCanvasGroup.DOKill();

        uiCanvasGroup.DOFade(targetAlpha, fadeTime)
            .SetUpdate(true)
            .OnComplete(() => {
                uiCanvasGroup.blocksRaycasts = (targetAlpha > 0.1f);
            });
    }

    /// <summary>
    /// 添加滑块事件监听
    /// </summary>
    private void AddUIEventListeners()
    {
        EventTrigger trigger = angleSlider.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = angleSlider.gameObject.AddComponent<EventTrigger>();

        // 滑块按下时
        EventTrigger.Entry pointerDownEntry = new EventTrigger.Entry();
        pointerDownEntry.eventID = EventTriggerType.PointerDown;
        pointerDownEntry.callback.AddListener((data) => { lastInteractionTime = Time.time; });
        trigger.triggers.Add(pointerDownEntry);

        // 滑块拖动时
        EventTrigger.Entry dragEntry = new EventTrigger.Entry();
        dragEntry.eventID = EventTriggerType.Drag;
        dragEntry.callback.AddListener((data) => { lastInteractionTime = Time.time; });
        trigger.triggers.Add(dragEntry);

        // 滑块释放时
        EventTrigger.Entry pointerUpEntry = new EventTrigger.Entry();
        pointerUpEntry.eventID = EventTriggerType.PointerUp;
        pointerUpEntry.callback.AddListener((data) => { lastInteractionTime = Time.time; });
        trigger.triggers.Add(pointerUpEntry);
    }

    private void OnAngleChanged(float value)
    {
        if (cameraController != null)
        {
            // 吸附逻辑
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

            if (closestDistance < snapThreshold)
            {
                value = closestValue;
                angleSlider.value = value;
            }

            cameraController.angle_x = value;
            lastInteractionTime = Time.time; // 操作时刷新显示时间
        }
    }
}