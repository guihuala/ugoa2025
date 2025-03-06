using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public float shakeMagnitude = 0.05f;  // 震动幅度
    public float shakeFrequency = 1.0f;   // 震动频率
    public float transitionTime = 0.5f;   // 切换位置的过渡时间

    private float timeOffsetX;
    private float timeOffsetY;

    public Vector3[] fixedPositions; // 预设的四个固定位置

    private int currentIndex = 1; // 当前相机所在的索引
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private float dragThreshold = 50f; // 判断滑动的最小距离
    private Vector3 currentTargetPosition; // 记录当前相机平滑移动的目标位置
    private bool isMoving = false; // 是否正在平滑移动

    private void Start()
    {
        timeOffsetX = Random.Range(0f, 100f);
        timeOffsetY = Random.Range(0f, 100f);

        // 设置当前目标位置
        currentTargetPosition = fixedPositions[currentIndex];
        
        Vector3 startPosition = fixedPositions[1];
        transform.position = startPosition;

        transform.DOMove(currentTargetPosition, transitionTime)
            .SetEase(Ease.OutQuad)
            .OnComplete(() => isMoving = false); // 移动完成后允许用户滑动
    }


    private void Update()
    {
        // 计算相机震动
        float xShake = Mathf.PerlinNoise(Time.time * shakeFrequency + timeOffsetX, 0) * 2f - 1f;
        float yShake = Mathf.PerlinNoise(0, Time.time * shakeFrequency + timeOffsetY) * 2f - 1f;
        xShake *= shakeMagnitude;
        yShake *= shakeMagnitude;
        
        transform.position = currentTargetPosition + new Vector3(xShake, yShake, 0f);

        HandleSwipe();
    }

    private void HandleSwipe()
    {
        if (isMoving) return; // 如果正在平滑移动，不接受新的滑动输入

        // 处理鼠标拖动
        if (Input.GetMouseButtonDown(0))
        {
            isDragging = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0) && isDragging)
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            if (Mathf.Abs(mouseDelta.x) > dragThreshold)
            {
                if (mouseDelta.x < 0) MoveRight();
                else MoveLeft();
            }

            isDragging = false;
        }

        // 处理触摸滑动
        if (Input.touchCount == 1)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                isDragging = true;
                lastMousePosition = touch.position;
            }

            if (touch.phase == TouchPhase.Ended && isDragging)
            {
                Vector3 touchDelta = (Vector3)touch.position - lastMousePosition;

                if (Mathf.Abs(touchDelta.x) > dragThreshold)
                {
                    if (touchDelta.x < 0) MoveRight();
                    else MoveLeft();
                }

                isDragging = false;
            }
        }
    }

    private void MoveLeft()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            MoveToPosition(currentIndex);
        }
    }

    private void MoveRight()
    {
        if (currentIndex < fixedPositions.Length - 1)
        {
            currentIndex++;
            MoveToPosition(currentIndex);
        }
    }

    private void MoveToPosition(int index)
    {
        isMoving = true; // 标记正在移动
        currentTargetPosition = fixedPositions[index]; // 目标位置更新
        
        transform.DOMove(currentTargetPosition, transitionTime)
                 .SetEase(Ease.OutQuad)
                 .OnComplete(() => isMoving = false); // 移动完成后解除锁定
    }
}
