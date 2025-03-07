using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class PerspectiveCameraController : MonoBehaviour , CameraController
{
    public Transform player;
    public float followSpeed = 2f;

    [Header("角度")] public float angle_x = 30f;
    public float angle_y = 45f;
    private Quaternion targetRotation;
    
    private float minAngleX = 10f;  // 最小角度
    private float maxAngleX = 60f;  // 最大角度
    private float rotationSpeed = 1f; // 旋转灵敏度

    [Header("缩放调节")] public float zoomSpeed = 6f;
    public float smoothZoomTime = 0.2f; // 缩放的平滑时间

    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector3 dragOrigin;
    private float targetZoom; // 目标缩放值
    private float zoomVelocity; // 用于平滑插值的临时变量

    private bool isZooming = false; // 用来控制是否正在缩放，防止多次触发

    private Camera mainCamera;
    private bool isShaking = false; // 是否正在震动
    private Vector3 shakeOffset = Vector3.zero; // 震动偏移量

    [Header("相机震动配置")]
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main; // 获取主相机
        mainCamera.orthographic = false; // 确保相机使用透视模式
        
        EVENTMGR.OnPlayerFound += ShakeCamera;
        
        targetRotation = Quaternion.Euler(angle_x, angle_y, 0f);
        transform.rotation = targetRotation;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnPlayerFound -= ShakeCamera;
    }

    public float rotationSmoothTime = 0.2f; // 平滑时间
    private float rotationVelocity; 

    void LateUpdate()
    {
        if (isShaking)
        {
            transform.position += shakeOffset;
        }

        if (Time.timeScale == 0 || isZooming || isShaking)
            return;

        HandleZoom();
        HandleDrag();
        HandleRotation(); // 处理旋转

        if (player == null || isDragging || isShaking) return;

        FollowPlayer();
    }


    // 跟随玩家
    void FollowPlayer()
    {
        if (player == null) return;

        float angleRadians_x = angle_x * Mathf.Deg2Rad;
        float angleRadians_y = angle_y * Mathf.Deg2Rad;

        float distance = 13f; // 相机与目标的距离（透视相机）

        float offsetX = Mathf.Cos(angleRadians_y) * Mathf.Cos(angleRadians_x) * distance;
        float offsetZ = Mathf.Sin(angleRadians_y) * Mathf.Cos(angleRadians_x) * distance;
        float offsetY = Mathf.Sin(angleRadians_x) * distance;

        // 确定相机目标位置
        Vector3 targetPosition = player.position - new Vector3(offsetX, -offsetY, offsetZ);

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        transform.rotation = Quaternion.Euler(angle_x, angle_y, 0f);
    }

    void HandleDrag()
    {
        if (IsTouchInput())
        {
            if (Input.touchCount == 1) // 处理触摸屏单指拖动
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Began)
                {
                    isDragging = true;
                    dragOrigin = touch.position;
                }

                if (touch.phase == TouchPhase.Moved && isDragging)
                {
                    Vector3 currentTouchPosition = touch.position;
                    Vector3 dragDifference = currentTouchPosition - dragOrigin;
                    dragOrigin = currentTouchPosition;

                    Vector3 worldDrag = new Vector3(-dragDifference.x, 0, -dragDifference.y);

                    float dragFactor = Mathf.Abs(transform.position.y) / Screen.height * 0.5f;

                    worldDrag *= dragFactor;

                    Vector3 adjustedDrag = Quaternion.Euler(0, angle_y, 0) * worldDrag;
                    transform.position += adjustedDrag;
                }

                if (touch.phase == TouchPhase.Ended)
                {
                    isDragging = false;
                }
            }
        }
        else // PC端处理鼠标拖动
        {
            if (Input.GetMouseButtonDown(0))
            {
                isDragging = true;
                dragOrigin = Input.mousePosition;
            }

            if (Input.GetMouseButton(0) && isDragging)
            {
                Vector3 currentMousePosition = Input.mousePosition;
                Vector3 dragDifference = currentMousePosition - dragOrigin;
                dragOrigin = currentMousePosition;

                Vector3 worldDrag = new Vector3(-dragDifference.x, 0, -dragDifference.y);

                float dragFactor = Mathf.Abs(transform.position.y) / Screen.height * 0.5f;

                worldDrag *= dragFactor;

                Vector3 adjustedDrag = Quaternion.Euler(0, angle_y, 0) * worldDrag;
                transform.position += adjustedDrag;
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
            }
        }
    }

    void HandleZoom()
    {
        if (IsTouchInput()) // 移动端触摸缩放
        {
            if (Input.touchCount == 2) // 如果有两根手指进行缩放
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                float previousDistance = (touch1.position - touch2.position).magnitude;
                float currentDistance = (touch1.position - touch2.position).magnitude;

                float deltaDistance = previousDistance - currentDistance;
                targetZoom += deltaDistance * zoomSpeed * 0.1f;
                targetZoom = Mathf.Clamp(targetZoom, 20f, 40f); // 透视相机的视野角度范围
                isZooming = true;
                mainCamera.fieldOfView = Mathf.SmoothDamp(
                    mainCamera.fieldOfView,
                    targetZoom,
                    ref zoomVelocity,
                    smoothZoomTime,
                    Mathf.Infinity, // 无需限制速度
                    Time.unscaledDeltaTime // 使用不受时间缩放影响的增量时间
                );
                isZooming = false;
            }
        }
        else // PC端滚轮缩放
        {
            float scrollInput = Input.GetAxis("Mouse ScrollWheel");
            targetZoom -= scrollInput * zoomSpeed;
            targetZoom = Mathf.Clamp(targetZoom, 20f, 40f); // 透视相机的视野角度范围

            isZooming = true;
            mainCamera.fieldOfView = Mathf.SmoothDamp(
                mainCamera.fieldOfView,
                targetZoom,
                ref zoomVelocity,
                smoothZoomTime,
                Mathf.Infinity, // 无需限制速度
                Time.unscaledDeltaTime // 使用不受时间缩放影响的增量时间
            );
            isZooming = false;
        }
    }
    
    void HandleRotation()
    {
        if (IsTouchInput()) // 触摸设备：双指滑动调整角度
        {
            if (Input.touchCount == 2)
            {
                Touch touch1 = Input.GetTouch(0);
                Touch touch2 = Input.GetTouch(1);

                // 计算两个手指的平均位置
                Vector2 touchCenter = (touch1.position + touch2.position) / 2;
            
                // 计算手指滑动方向
                float deltaY = (touch1.deltaPosition.y + touch2.deltaPosition.y) / 2;
            
                // 调整 x 轴角度
                angle_x -= deltaY * rotationSpeed;
                angle_x = Mathf.Clamp(angle_x, minAngleX, maxAngleX);
            
                // 应用旋转
                UpdateCameraAngle();
            }
        }
        else // 电脑端鼠标右键拖动
        {
            if (Input.GetMouseButton(1)) // 右键按住
            {
                float deltaY = Input.GetAxis("Mouse Y"); // 读取鼠标垂直移动
            
                // 计算新的 x 角度
                angle_x -= deltaY * rotationSpeed * 10f;
                angle_x = Mathf.Clamp(angle_x, minAngleX, maxAngleX);

                // 应用旋转
                UpdateCameraAngle();
            }
        }
    }
    
    public void UpdateCameraAngle()
    {
        Quaternion targetRotation = Quaternion.Euler(angle_x, angle_y, 0f);
        transform.DORotateQuaternion(targetRotation, 0.2f).SetEase(Ease.OutQuad);
    }

    public void SetCameraZoom(float targetSize)
    {
        targetZoom = targetSize;
        isZooming = true; // 设置为正在缩放
        mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetZoom,
            ref zoomVelocity,
            0.1f,
            Mathf.Infinity, // 无需限制速度
            Time.unscaledDeltaTime // 使用不受时间缩放影响的增量时间
        );
        isZooming = false; // 完成缩放后设置为未缩放
    }
    

    // 判断当前设备是否是触摸设备
    private bool IsTouchInput()
    {
        return Input.touchCount > 0;
    }

    // 震动效果方法
    public void ShakeCamera()
    {
        if (isShaking) return; // 防止多次震动同时发生
        
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
