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
        Vector3 playerPosition = player.position;
        float angleRadians_x = angle_x * Mathf.Deg2Rad;
        float angleRadians_y = angle_y * Mathf.Deg2Rad;
        
        Vector3 targetPosition = new Vector3(
            playerPosition.x - Mathf.Cos(angleRadians_y) * 13f,
            transform.position.y, // 高度固定
            playerPosition.z - Mathf.Cos(angleRadians_x) * 10f
        );
        
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 1f / followSpeed);
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

    
    // 左键拖拽
    void HandleDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            dragOrigin = Input.mousePosition;
        }

        if (Input.GetMouseButton(0) && isDragging)
        {
            Vector3 dragDifference = Input.mousePosition - dragOrigin;
            dragOrigin = Input.mousePosition;


            Vector3 move = new Vector3(-dragDifference.x * dragSpeed * Time.deltaTime, 0, -dragDifference.y * dragSpeed * Time.deltaTime);
            transform.Translate(move, Space.World);
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }
}
