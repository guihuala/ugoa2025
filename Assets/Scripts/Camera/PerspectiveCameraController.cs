using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerspectiveCameraController : MonoBehaviour , CameraController
{
    public Transform player;
    public float followSpeed = 2f;

    [Header("角度")] public float angle_x = 30f;
    public float angle_y = 45f;

    [Header("缩放调节")] public float zoomSpeed = 6f;
    public float smoothZoomTime = 0.2f; // 缩放的平滑时间

    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector3 dragOrigin;
    private float targetZoom; // 目标缩放值
    private float zoomVelocity; // 用于平滑插值的临时变量

    private bool isZooming = false; // 用来控制是否正在缩放，防止多次触发

    private Camera mainCamera;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = Camera.main; // 获取主相机
        mainCamera.orthographic = false; // 确保相机使用透视模式
    }

    void LateUpdate()
    {
        if (Time.timeScale == 0 || isZooming) // 如果正在缩放或者游戏暂停，则不进行相机更新
            return;

        HandleZoom(); // 处理滚轮缩放
        HandleDrag();

        if (player == null || isDragging) return; // 拖动时不跟随玩家

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

    void HandleZoom()
    {
        // 获取滚轮输入并更新目标缩放值
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, 20f, 40f); // 透视相机的视野角度范围
        
        isZooming = true; // 设置为正在缩放
        mainCamera.fieldOfView = Mathf.SmoothDamp(
            mainCamera.fieldOfView,
            targetZoom,
            ref zoomVelocity,
            smoothZoomTime,
            Mathf.Infinity, // 无需限制速度
            Time.unscaledDeltaTime // 使用不受时间缩放影响的增量时间
        );
        isZooming = false; // 缩放结束后设置为未缩放
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
}
