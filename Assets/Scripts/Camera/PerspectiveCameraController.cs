using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
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

    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector2 dragOrigin; // 平移时的起始点

    // 用于旋转输入的标志和起始点（仅用于修改 x 轴）
    private bool isRotatingInput = false;
    private Vector2 rotationDragOrigin;

    private float targetZoom; // 目标缩放值
    private float zoomVelocity; // 用于平滑插值的临时变量
    private bool isZooming = false; // 缩放标志

    private Camera mainCamera;
    private bool isShaking = false; // 震动标志
    private Vector3 shakeOffset = Vector3.zero; // 震动偏移量

    [Header("相机震动配置")]
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;

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

    void LateUpdate()
    {
        if (isShaking)
        {
            transform.position += shakeOffset;
        }

        if (Time.timeScale == 0 || isZooming || isShaking)
            return;

        HandleZoom();
        HandleInput();  // 处理平移和旋转输入

        if (player == null || isDragging || isRotatingInput || isShaking)
            return;

        FollowPlayer();
    }

    // 以玩家为中心计算相机位置，并始终面向玩家
    void FollowPlayer()
    {
        if (player == null) return;

        float distance = 13f; // 相机与玩家的距离

        // 将角度转换为弧度
        float radX = angle_x * Mathf.Deg2Rad;
        float radY = angle_y * Mathf.Deg2Rad;
        
        // 采用球坐标计算偏移量
        float offsetX = distance * Mathf.Cos(radX) * Mathf.Sin(radY);
        float offsetY = distance * Mathf.Sin(radX);
        float offsetZ = distance * Mathf.Cos(radX) * Mathf.Cos(radY);
        
        // 以玩家为中心
        Vector3 targetPosition = player.position + new Vector3(offsetX, offsetY, offsetZ);
        
        // 平滑移动相机
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        
        // 始终面向玩家
        transform.LookAt(player);
    }

    // 处理平移与旋转输入
    void HandleInput()
    {
        if (IsTouchInput())
        {
            // 当只有一根手指时，区分平移与旋转
            if (Input.touchCount == 1)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    // 屏幕右侧 80% 作为旋转输入区域
                    if (touch.position.x > Screen.width * 0.8f)
                    {
                        isRotatingInput = true;
                        rotationDragOrigin = touch.position;
                    }
                    else
                    {
                        isDragging = true;
                        dragOrigin = touch.position;
                    }
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (isRotatingInput)
                    {
                        Vector2 currentPos = touch.position;
                        Vector2 delta = currentPos - rotationDragOrigin;
                        rotationDragOrigin = currentPos;
                        
                        // 仅修改 x 轴（俯仰角），忽略水平方向的变化
                        angle_x -= delta.y * rotationSpeed * 10f;
                        angle_x = Mathf.Clamp(angle_x, minAngleX, maxAngleX);
                    }
                    else if (isDragging)
                    {
                        Vector2 currentPos = touch.position;
                        Vector2 delta = currentPos - dragOrigin;
                        dragOrigin = currentPos;

                        Vector3 worldDrag = new Vector3(-delta.x, 0, -delta.y);
                        float dragFactor = Mathf.Abs(transform.position.y) / Screen.height * 0.5f;
                        worldDrag *= dragFactor;
                        Vector3 adjustedDrag = Quaternion.Euler(0, angle_y, 0) * worldDrag;
                        transform.position -= adjustedDrag;
                    }
                }
                else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    isRotatingInput = false;
                    isDragging = false;
                }
            }
            // 两指触控用于缩放
            else if (Input.touchCount == 2)
            {
                HandleZoom();
            }
        }
        else // PC端输入
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (Input.mousePosition.x > Screen.width * 0.8f)
                {
                    isRotatingInput = true;
                    rotationDragOrigin = Input.mousePosition;
                }
                else
                {
                    isDragging = true;
                    dragOrigin = Input.mousePosition;
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (isRotatingInput)
                {
                    Vector2 currentPos = Input.mousePosition;
                    Vector2 delta = currentPos - rotationDragOrigin;
                    rotationDragOrigin = currentPos;

                    // 仅根据垂直方向更新俯仰角
                    angle_x -= delta.y * rotationSpeed * 10f;
                    angle_x = Mathf.Clamp(angle_x, minAngleX, maxAngleX);
                }
                else if (isDragging)
                {
                    Vector2 currentPos = Input.mousePosition;
                    Vector2 delta = currentPos - dragOrigin;
                    dragOrigin = currentPos;

                    Vector3 worldDrag = new Vector3(-delta.x, 0, -delta.y);
                    float dragFactor = Mathf.Abs(transform.position.y) / Screen.height * 0.5f;
                    worldDrag *= dragFactor;
                    Vector3 adjustedDrag = Quaternion.Euler(0, angle_y, 0) * worldDrag;
                    transform.position -= adjustedDrag;
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isRotatingInput = false;
                isDragging = false;
            }
        }
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
                targetZoom = Mathf.Clamp(targetZoom, 20f, 40f);
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
                targetZoom = Mathf.Clamp(targetZoom, 20f, 40f);
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

    // 震动效果方法
    public void ShakeCamera()
    {
        if (isShaking) return;
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    // 震动协程
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
}
