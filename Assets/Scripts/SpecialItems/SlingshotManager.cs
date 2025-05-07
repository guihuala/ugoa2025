using UnityEngine;
using UnityEngine.EventSystems;


public class SlingshotManager : MonoBehaviour
{
    [Header("发射设置")]
    public float launchForce = 10f;         // 固定发射力量
    public float maxDragDistance = 50f;     // 最大拖拽距离
    public float velocityInfluenceFactor = 0.5f; // 人物速度对发射的影响系数
    public float minDragDistance = 10f;     // 最小有效拖拽距离
    [Range(0, 90)] public float maxVerticalAngle = 45f; // 最大垂直发射角度(度)

    [Header("视觉效果")]
    public LineRenderer trajectoryLineRenderer; // 轨迹线渲染器
    public int trajectoryPoints = 20;       // 轨迹线点数
    public float trajectoryTimeStep = 0.1f; // 轨迹预测时间步长

    [Header("参考点")]
    public Transform slingStartPoint;       // 弹弓起始点
    public Transform characterTransform;    // 人物Transform
    public BulletPool bulletPool;           // 子弹对象池

    private Vector3 characterVelocity;      // 人物当前速度
    private Vector3 slingBaseLocalPosition; // 弹弓相对于人物的本地位置
    private Vector3 dragStartWorldPosition; // 拖拽开始的世界坐标
    private bool isDragging = false;        // 是否正在拖拽
    private bool isUsingSlingshot = false;  // 是否正在使用弹弓

    private void Start()
    {
        slingBaseLocalPosition = slingStartPoint.localPosition;
        
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = 0;
        }
    }

    private void Update()
    {
        if (!isUsingSlingshot) return;

        // 更新弹弓位置(始终跟随人物)
        slingStartPoint.position = characterTransform.TransformPoint(slingBaseLocalPosition);

        // 更新人物速度
        UpdateCharacterVelocity();

        HandleInput();
    }

    private void UpdateCharacterVelocity()
    {
        if (characterTransform.TryGetComponent<Rigidbody>(out var rb))
        {
            characterVelocity = rb.velocity;
        }
        else
        {
            characterVelocity = (characterTransform.position - lastCharacterPosition) / Time.deltaTime;
            lastCharacterPosition = characterTransform.position;
        }
    }
    private Vector3 lastCharacterPosition;

    private void HandleInput()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        if (Input.GetMouseButtonDown(0))
        {
            StartDrag();
        }
        else if (Input.GetMouseButtonUp(0) && isDragging)
        {
            EndDrag();
        }

        if (isDragging)
        {
            UpdateDrag();
        }
    }

    private void StartDrag()
    {
        AudioManager.Instance.PlaySfx("TightenUp");
        
        isDragging = true;
        dragStartWorldPosition = slingStartPoint.position;
    }

    private void UpdateDrag()
    {
        // 获取当前鼠标位置(世界坐标)
        Vector3 currentDragPosition = GetMouseWorldPosition();
        
        // 计算拖拽向量并限制最大距离
        Vector3 dragVector = currentDragPosition - dragStartWorldPosition;
        
        // 限制垂直角度
        dragVector = ClampVerticalAngle(dragVector, maxVerticalAngle);
        dragVector = Vector3.ClampMagnitude(dragVector, maxDragDistance);

        // 更新弹弓位置(临时偏移)
        slingStartPoint.position = dragStartWorldPosition + dragVector;

        // 更新轨迹预测
        UpdateTrajectory(dragVector);
    }

    private Vector3 ClampVerticalAngle(Vector3 direction, float maxAngle)
    {
        // 计算当前角度
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        float currentAngle = Vector3.Angle(horizontalDirection, direction);
        
        // 确定是向上还是向下
        float angleSign = Mathf.Sign(direction.y);
        
        // 如果角度超过限制，则调整y值
        if (Mathf.Abs(currentAngle) > maxAngle)
        {
            float horizontalLength = horizontalDirection.magnitude;
            float maxY = horizontalLength * Mathf.Tan(maxAngle * Mathf.Deg2Rad);
            direction.y = maxY * angleSign;
        }
        
        return direction;
    }

    private void EndDrag()
    {
        if (!isDragging) return;
        isDragging = false;
    
        // 计算当前鼠标位置
        Vector3 currentDragPosition = GetMouseWorldPosition();
    
        // 计算拖拽向量
        Vector3 dragVector = currentDragPosition - dragStartWorldPosition;
        dragVector = ClampVerticalAngle(dragVector, maxVerticalAngle);
        dragVector = Vector3.ClampMagnitude(dragVector, maxDragDistance);
    
        Debug.Log("Drag Vector: " + dragVector);
    
        // 检查是否达到最小拖拽距离
        if (dragVector.magnitude >= minDragDistance)
        {
            AudioManager.Instance.PlaySfx("FlyOut");
            Vector3 launchDirection = -dragVector.normalized;
            LaunchBullet(launchDirection);
        }

        // 重置弹弓位置
        slingStartPoint.localPosition = slingBaseLocalPosition;
    
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = 0;
        }

        TerminateSlingshot();
    }

    private Vector3 GetMouseWorldPosition()
    {
        // 创建射线从相机到鼠标位置
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
        // 计算射线与垂直于Y轴的平面的交点
        float enter;
        if (new Plane(Vector3.up, dragStartWorldPosition).Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
    
        // 如果射线与平面不相交，返回默认值
        return dragStartWorldPosition;
    }
    
    private void LaunchBullet(Vector3 launchDirection)
    {
        // 固定发射力度，只受人物速度影响
        Vector3 finalForce = launchDirection * launchForce + (characterVelocity * velocityInfluenceFactor);

        // 从对象池获取子弹
        GameObject bullet = bulletPool.GetBullet(slingStartPoint.position, Quaternion.identity);
    
        // 配置子弹
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero; // 重置速度
        rb.angularVelocity = Vector3.zero; // 重置角速度
        rb.AddForce(finalForce, ForceMode.VelocityChange);
    
        // 激活子弹相关组件
        bullet.SetActive(true);
    }

    private void UpdateTrajectory(Vector3 dragVector)
    {
        if (trajectoryLineRenderer == null) return;

        Vector3 launchDirection = -dragVector.normalized;
        Vector3 initialVelocity = launchDirection * launchForce + (characterVelocity * velocityInfluenceFactor);

        Vector3[] points = new Vector3[trajectoryPoints];
        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * trajectoryTimeStep;
            Vector3 predictedCharacterPos = characterTransform.position + characterVelocity * t;
            Vector3 bulletMotion = initialVelocity * t + 0.5f * Physics.gravity * t * t;
            points[i] = predictedCharacterPos + bulletMotion;
        }

        trajectoryLineRenderer.positionCount = trajectoryPoints;
        trajectoryLineRenderer.SetPositions(points);
    }
    
    public void SetIsUsingSlingshot(bool isUsing)
    {
        isUsingSlingshot = isUsing;
        
        if (!isUsing)
        {
            slingStartPoint.localPosition = slingBaseLocalPosition;
        }
    }
    
    private void TerminateSlingshot()
    {
        SetIsUsingSlingshot(false);
        EVENTMGR.TriggerUsingSlingshot();
    }
}