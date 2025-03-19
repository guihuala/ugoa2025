using UnityEngine;

public class Slingshot : MonoBehaviour
{
    public GameObject bulletPrefab; // 子弹预制体
    public float launchForce = 10f; // 发射力量
    public float maxDragDistance = 50f; // 最大拖拽距离
    public LineRenderer lineRenderer; // 用于显示拖拽的线

    public Transform slingStart; // 弹弓的起始位置
    
    private Vector3 slingStartPosition; // 弹弓起始位置
    private Vector3 dragStartPosition; // 鼠标按下位置
    private bool isDragging = false; // 是否在拖拽

    private void Start()
    {
        slingStartPosition = slingStart.position;
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // 两个点：一端是弹弓起始位置，一端是当前拖拽位置
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))   // 鼠标左键按下
        {
            dragStartPosition = GetMouseWorldPosition(); // 获取鼠标在3D空间中的初始位置
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))  // 鼠标左键抬起
        {
            if (isDragging)
            {
                LaunchSlingshot();
                isDragging = false;
            }
        }

        if (isDragging)
        {
            DragSlingshot();
        }
    }
    
    Vector3 GetMouseWorldPosition()
    {
        // 创建一个3D向量，其中屏幕上的x和y坐标由Input.mousePosition获取，z坐标为弹弓起始位置的深度
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(slingStart.position).z;  // 获取与弹弓深度相同的z值

        // 转换为世界坐标
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    void DragSlingshot()
    {
        // 获取鼠标当前位置
        Vector3 currentDragPosition = GetMouseWorldPosition();

        currentDragPosition.y = 0;
        
        // 计算鼠标当前位置和起始位置的向量
        Vector3 dragVector = currentDragPosition - dragStartPosition;

        // 限制拖拽的最大距离
        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }

        slingStart.position = slingStartPosition + dragVector;

        // 绘制拖拽线
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, slingStartPosition);          // 设置线条的起点
            lineRenderer.SetPosition(1, slingStart.position);         // 设置线条的终点
        }
    }

    void LaunchSlingshot()
    {
        // 计算发射方向和力度
        Vector3 launchDirection = slingStartPosition - slingStart.position;
        launchDirection.y = 0f;

        // 计算发射力度（与拖动的距离成正比）
        float distance = launchDirection.magnitude;
        Vector3 force = launchDirection.normalized * launchForce * distance;

        // 创建并发射子弹
        GameObject bullet = Instantiate(bulletPrefab, slingStartPosition, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = force;

        // 重置弹弓位置
        slingStart.position = slingStartPosition;

        // 清除线条
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, slingStartPosition);
            lineRenderer.SetPosition(1, slingStartPosition);  // 发射后清空线
        }
    }
}
