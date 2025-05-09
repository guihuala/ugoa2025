using UnityEngine;
using UnityEngine.EventSystems;


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
    public float failedLaunchForce = 2f;    // 发射失败时的微弱力度

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
    private Vector3 lastCharacterPosition;

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
        Vector3 currentDragPosition = GetMouseWorldPosition();
        Vector3 dragVector = currentDragPosition - dragStartWorldPosition;
        dragVector = ClampVerticalAngle(dragVector, maxVerticalAngle);
        dragVector = Vector3.ClampMagnitude(dragVector, maxDragDistance);

        slingStartPoint.position = dragStartWorldPosition + dragVector;
        UpdateTrajectory(dragVector);
    }

    private Vector3 ClampVerticalAngle(Vector3 direction, float maxAngle)
    {
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        float currentAngle = Vector3.Angle(horizontalDirection, direction);
        float angleSign = Mathf.Sign(direction.y);
        
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
    
        Vector3 currentDragPosition = GetMouseWorldPosition();
        Vector3 dragVector = currentDragPosition - dragStartWorldPosition;
        dragVector = ClampVerticalAngle(dragVector, maxVerticalAngle);
        dragVector = Vector3.ClampMagnitude(dragVector, maxDragDistance);
    
        // 检查是否发射失败
        bool isFailedLaunch = dragVector.magnitude < minDragDistance || 
                            IsInvalidAngle(dragVector, maxVerticalAngle);
    
        if (isFailedLaunch)
        {
            // 发射失败，使用微弱力度下落
            AudioManager.Instance.PlaySfx("Drop");
            LaunchBullet(Vector3.down, failedLaunchForce);
        }
        else
        {
            // 正常发射
            AudioManager.Instance.PlaySfx("FlyOut");
            Vector3 launchDirection = -dragVector.normalized;
            LaunchBullet(launchDirection, launchForce);
        }

        // 重置弹弓位置
        slingStartPoint.localPosition = slingBaseLocalPosition;
    
        if (trajectoryLineRenderer != null)
        {
            trajectoryLineRenderer.positionCount = 0;
        }

        TerminateSlingshot();
    }

    // 检查角度是否无效
    private bool IsInvalidAngle(Vector3 direction, float maxAngle)
    {
        Vector3 horizontalDirection = new Vector3(direction.x, 0, direction.z);
        float currentAngle = Vector3.Angle(horizontalDirection, direction);
        return Mathf.Abs(currentAngle) > maxAngle;
    }

    private Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float enter;
        if (new Plane(Vector3.up, dragStartWorldPosition).Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        return dragStartWorldPosition;
    }
    
    private void LaunchBullet(Vector3 launchDirection, float force)
    {
        Vector3 finalForce = launchDirection * force + (characterVelocity * velocityInfluenceFactor);

        GameObject bullet = bulletPool.GetBullet(slingStartPoint.position, Quaternion.identity);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.AddForce(finalForce, ForceMode.VelocityChange);
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