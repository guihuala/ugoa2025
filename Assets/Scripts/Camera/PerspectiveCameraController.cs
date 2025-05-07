using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class PerspectiveCameraController : MonoBehaviour, CameraController
{
    public Transform player;
    public float followSpeed = 1f;

    [Header("角度")]
    public float angle_x = 30f;      // 俯仰角（允许修改）
    public float angle_y = 45f;      // 水平角（保持不变）
    private float minAngleX = 10f;   // 最小俯仰角
    private float maxAngleX = 60f;   // 最大俯仰角
    private float rotationSpeed = 0.01f; // 旋转灵敏度

    [Header("缩放调节")]
    public float zoomSpeed = 10;
    public float smoothZoomTime = 0.2f; // 缩放的平滑时间
    public float minZoom = 20f;
    public float maxZoom = 40f;
    
    [Header("相机震动配置")]
    [SerializeField] private float duration = 0.2f;
    [SerializeField] private float magnitude = 0.1f;

    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector2 dragOrigin; // 平移时的起始点

    private Vector2 rotationDragOrigin;

    private float targetZoom; // 目标缩放值
    private float zoomVelocity; // 用于平滑插值的临时变量
    private bool isZooming = false; // 缩放标志

    private Camera mainCamera;
    private bool isShaking = false; // 震动标志
    private Vector3 shakeOffset = Vector3.zero; // 震动偏移量
    
    [Header("是否开启触屏控制")]
    public bool allowCameraControl = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main;
        mainCamera.orthographic = false; // 使用透视模式

        EVENTMGR.OnPlayerFound += ShakeCamera;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnPlayerFound -= ShakeCamera;
    }

    void Update()
    {
        if (isShaking)
        {
            transform.position += shakeOffset;
        }

        if (Time.timeScale == 0 || isZooming || isShaking)
            return;

        HandleZoom();
        HandleInput();

        if (player == null || isDragging || isShaking)
            return;

        FollowPlayer();
    }

    void FollowPlayer()
    {
        if (player == null) return;

        float distance = 13f;

        float radX = angle_x * Mathf.Deg2Rad;
        float radY = angle_y * Mathf.Deg2Rad;

        float offsetX = distance * Mathf.Cos(radX) * Mathf.Sin(radY);
        float offsetY = distance * Mathf.Sin(radX);
        float offsetZ = distance * Mathf.Cos(radX) * Mathf.Cos(radY);

        Vector3 targetPosition = player.position + new Vector3(offsetX, offsetY, offsetZ);
    
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        
        Quaternion targetRotation = Quaternion.LookRotation(player.position - transform.position);
        transform.DORotateQuaternion(targetRotation, 1f).SetEase(Ease.OutCubic);
    }

    private void HandleInput()
    {
        if (!allowCameraControl)
            return;
        
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        Vector2? currentInput = null;
        bool began = false, ended = false;
        
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            currentInput = touch.position;
            began = (touch.phase == TouchPhase.Began);
            ended = (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled);
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                currentInput = Input.mousePosition;
                began = true;
            }
            else if (Input.GetMouseButton(0))
            {
                currentInput = Input.mousePosition;
            }
            else if (Input.GetMouseButtonUp(0))
            {
                ended = true;
            }
        }

        if (currentInput.HasValue)
        {
            if (began)
            {
                isDragging = true;
                dragOrigin = currentInput.Value;
            }
            else if (isDragging)
            {
                Vector2 delta = currentInput.Value - dragOrigin;
                dragOrigin = currentInput.Value;
                ProcessDrag(delta);
            }
        }
        if (ended)
        {
            isDragging = false;
        }

        // 缩放处理：当移动端双指或PC端鼠标滚轮操作时调用
        if (Input.touchCount == 2 || Math.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0.001f)
        {
            HandleZoom();
        }
    }

    private void ProcessDrag(Vector2 delta)
    {
        Vector3 worldDrag = new Vector3(-delta.x, 0, -delta.y);
        float dragFactor = Mathf.Abs(transform.position.y) / Screen.height * 0.5f;
        worldDrag *= dragFactor;
        Vector3 adjustedDrag = Quaternion.Euler(0, angle_y, 0) * worldDrag;
        transform.position -= adjustedDrag;
    }

    void HandleZoom()
    {
        if (IsTouchInput()) // 移动端两指缩放
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                float previousDistance = (touch1.position - touch2.position).magnitude;
                float currentDistance = (touch1.position - touch2.position).magnitude;
                float deltaDistance = previousDistance - currentDistance;

                targetZoom += deltaDistance * zoomSpeed * 0.1f;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
                isZooming = true;
                mainCamera.fieldOfView = Mathf.SmoothDamp(
                    mainCamera.fieldOfView,
                    targetZoom,
                    ref zoomVelocity,
                    smoothZoomTime,
                    Mathf.Infinity,
                    Time.unscaledDeltaTime
                );
                isZooming = false;
            }
        }
        else // PC端鼠标滚轮缩放
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scrollInput) > 0.001f)
            {
                targetZoom -= scrollInput * zoomSpeed;
                targetZoom = Mathf.Clamp(targetZoom, minZoom, maxZoom);
                isZooming = true;
                mainCamera.fieldOfView = Mathf.SmoothDamp(
                    mainCamera.fieldOfView,
                    targetZoom,
                    ref zoomVelocity,
                    smoothZoomTime,
                    Mathf.Infinity,
                    Time.unscaledDeltaTime
                );
                isZooming = false;
            }
        }
    }

    public void SetCameraZoom(float targetSize)
    {
        targetZoom = targetSize;
        isZooming = true;
        mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetZoom,
            ref zoomVelocity,
            0.1f,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );
        isZooming = false;
    }

    // 判断当前设备是否为触摸设备
    private bool IsTouchInput()
    {
        return Input.touchCount > 0;
    }

    #region 震动

    private void ShakeCamera()
    {
        if (isShaking) return;
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void ShakeCamera(float duration)
    {
        if (isShaking) return;
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }
    
    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        isShaking = true;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            shakeOffset = Random.insideUnitSphere * magnitude;
            elapsed += Time.deltaTime;
            yield return null;
        }

        shakeOffset = Vector3.zero;
        isShaking = false;
    }    

    #endregion
}