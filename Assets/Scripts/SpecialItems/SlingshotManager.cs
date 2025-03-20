using UnityEngine;
using UnityEngine.EventSystems;

public class SlingshotManager : MonoBehaviour
{
    public float launchForce = 10f;         // 发射力量
    public float maxDragDistance = 50f;       // 最大拖拽距离
    public LineRenderer lineRenderer;         // 用于显示拖拽的线

    public LineRenderer trajectoryLineRenderer; // 用于显示子弹轨迹的线
    public int trajectoryPoints = 10;           // 轨迹线点数
    public float timeStep = 0.5f;               // 每个轨迹点间的时间间隔

    public Transform slingStart;             // 弹弓的起始位置
    
    public BulletPool bulletPool;

    private bool isUsingSlingshot = false;
    
    private Vector3 slingStartPosition;      // 弹弓初始位置
    private Vector3 dragStartPosition;       // 鼠标按下位置
    private bool isDragging = false;         // 是否在拖拽

    private void Start()
    {
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2;
        }
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = 0; // 初始不显示轨迹
        }
    }

    void Update()
    {
        if (!isUsingSlingshot)
        {
            slingStartPosition = slingStart.position;
            return;
        }

        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = GetMouseWorldPosition();
            isDragging = true;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            if (isDragging)
            {
                LaunchSlingshot();
                isDragging = false;
                if (trajectoryLineRenderer != null)
                {
                    trajectoryLineRenderer.positionCount = 0;
                }
            }
        }

        if (isDragging)
        {
            DragSlingshot();
            DrawTrajectory();
        }
    }

    public void SetIsUsingSlingshot(bool isUsingSlingshot)
    {
        this.isUsingSlingshot = isUsingSlingshot;
    }
    
    Vector3 GetMouseWorldPosition()
    {
        // 获取鼠标屏幕位置，并设置与弹弓相同的深度
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.WorldToScreenPoint(slingStart.position).z;
        return Camera.main.ScreenToWorldPoint(mouseScreenPosition);
    }

    void DragSlingshot()
    {
        // 获取当前鼠标位置
        Vector3 currentDragPosition = GetMouseWorldPosition();
        currentDragPosition.y = 0;

        // 计算拖拽向量并限制最大距离
        Vector3 dragVector = currentDragPosition - dragStartPosition;
        if (dragVector.magnitude > maxDragDistance)
        {
            dragVector = dragVector.normalized * maxDragDistance;
        }
        slingStart.position = slingStartPosition + dragVector;

        // 绘制拖拽线
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, slingStartPosition);
            lineRenderer.SetPosition(1, slingStart.position);
        }
    }

    void LaunchSlingshot()
    {
        // 计算发射方向和力度
        Vector3 launchDirection = slingStartPosition - slingStart.position;
        launchDirection.y = 0f; // 保持水平发射
        float distance = launchDirection.magnitude;
        Vector3 force = launchDirection.normalized * launchForce * distance;

        // 使用对象池获取子弹
        GameObject bullet = bulletPool.GetBullet(slingStartPosition, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.velocity = force;

        // 重置弹弓位置
        slingStart.position = slingStartPosition;

        // 清除拖拽线
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, slingStartPosition);
            lineRenderer.SetPosition(1, slingStartPosition);
        }
    }

    void DrawTrajectory()
    {
        if (trajectoryLineRenderer == null)
            return;

        // 根据当前拖拽位置计算出发射时的初始速度
        Vector3 launchDirection = slingStartPosition - slingStart.position;
        launchDirection.y = 0f; // 水平速度分量
        float distance = launchDirection.magnitude;
        Vector3 initialVelocity = launchDirection.normalized * launchForce * distance;

        // 计算轨迹点位置：因为不受重力影响，轨迹为直线运动：位置 = 初始位置 + 速度 * t
        Vector3[] points = new Vector3[trajectoryPoints];
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeStep;
            points[i] = slingStartPosition + initialVelocity * t;
        }

        trajectoryLineRenderer.positionCount = trajectoryPoints;
        trajectoryLineRenderer.SetPositions(points);
    }
}