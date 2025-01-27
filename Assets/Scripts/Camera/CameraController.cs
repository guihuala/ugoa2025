using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class CameraController : MonoBehaviour
{
    public Transform player;
    public float followSpeed = 5f;
    
    [Header("角度")]
    public float angle_x = 25f;
    public float angle_y = 45f;
    
    [Header("缩放调节")]
    public float zoomSpeed = 6f;
    public float dragSpeed = 2f;
    public float smoothZoomTime = 0.2f; // 缩放的平滑时间

    private Vector3 velocity = Vector3.zero;
    private bool isDragging = false;
    private Vector3 dragOrigin;
    private float targetZoom; // 目标缩放值
    private float zoomVelocity; // 用于平滑插值的临时变量

    void LateUpdate()
    {
        HandleZoom(); // 处理缩放
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
        
        float offsetX = Mathf.Cos(angleRadians_y) * Mathf.Cos(angleRadians_x) * 13f;
        float offsetZ = Mathf.Sin(angleRadians_y) * Mathf.Cos(angleRadians_x) * 13f;
        float offsetY = Mathf.Sin(angleRadians_x) * 13f;

        // 确定相机目标位置
        Vector3 targetPosition = player.position - new Vector3(offsetX, -offsetY, offsetZ);
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
        transform.rotation = Quaternion.Euler(angle_x, angle_y, 0f);
    }

    // 平滑滚轮缩放
    void HandleZoom()
    {
        // 获取滚轮输入并更新目标缩放值
        float scrollInput = Input.GetAxis("Mouse ScrollWheel");
        targetZoom -= scrollInput * zoomSpeed;
        targetZoom = Mathf.Clamp(targetZoom, 3, 4.5f);

        // 平滑插值到目标缩放值
        Camera.main.orthographicSize = Mathf.SmoothDamp(
            Camera.main.orthographicSize,
            targetZoom,
            ref zoomVelocity,
            smoothZoomTime
        );
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

        // 将屏幕坐标的拖拽差异转换为世界坐标
        Vector3 worldDrag = new Vector3(-dragDifference.x, 0, -dragDifference.y);
        float dragFactor = (Camera.main.orthographic 
                            ? Camera.main.orthographicSize / Screen.height * 2f 
                            : Mathf.Abs(transform.position.y) / Screen.height * 2f);

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
