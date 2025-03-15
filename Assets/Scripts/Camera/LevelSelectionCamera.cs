using UnityEngine;
using System.Collections;
using DG.Tweening;
using UnityEngine.UI;

public class LevelSelectionCamera : MonoBehaviour
{
    [Header("相机基本配置")]
    public float shakeMagnitude = 0.05f;  // 震动幅度
    public float shakeFrequency = 1.0f;   // 震动频率
    public float transitionTime = 0.5f;   // 切换位置的过渡时间

    [Header("组件配置")]
    public Button prevBtn;
    public Button nextBtn;
    public Vector3[] fixedPositions; // 预设的四个固定位置
    
    [Header("材质配置")]
    public Material skyboxMaterial;
    public Color startColor0;
    public Color startColor1;
    public Color[] targetColor0;
    public Color[] targetColor1;
    
    private float timeOffsetX;
    private float timeOffsetY;
    
    private int currentIndex = 1; // 当前相机所在的索引
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private float dragThreshold = 50f; // 判断滑动的最小距离
    private Vector3 currentTargetPosition; // 记录当前相机平滑移动的目标位置
    private bool isMoving = false;

    private void Start()
    {
        prevBtn.onClick.AddListener(MoveLeft);
        nextBtn.onClick.AddListener(MoveRight);
        
        timeOffsetX = Random.Range(0f, 100f);
        timeOffsetY = Random.Range(0f, 100f);

        // 设置当前目标位置
        currentTargetPosition = fixedPositions[currentIndex];

        // 让相机从固定的初始位置平滑移动到目标位置
        Vector3 startPosition = fixedPositions[1]; 
        transform.position = startPosition;

        StartCoroutine(SmoothMove(startPosition, currentTargetPosition, transitionTime));
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
        if (isMoving) return; // 防止重复调用
        isMoving = true;
        Vector3 newTarget = fixedPositions[index];

        StartCoroutine(SmoothMove(transform.position, newTarget, transitionTime));
    }

    private IEnumerator SmoothMove(Vector3 startPos, Vector3 endPos, float duration)
    {
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            // 平滑插值相机位置
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);

            Color newColor0 = Color.Lerp(startColor0, targetColor0[currentIndex], t);
            Color newColor1 = Color.Lerp(startColor1, targetColor1[currentIndex], t);
            skyboxMaterial.SetColor("_Color0", newColor0);
            skyboxMaterial.SetColor("_Color1", newColor1);
            
            transform.position = newPos;
        
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        // 确保位置和颜色完全更新到目标值
        transform.position = endPos;
        skyboxMaterial.SetColor("_Color0", targetColor0[currentIndex]);
        skyboxMaterial.SetColor("_Color1", targetColor1[currentIndex]);
        // 更新起始颜色，便于下次过渡使用
        startColor0 = targetColor0[currentIndex];
        startColor1 = targetColor1[currentIndex];
    
        currentTargetPosition = endPos;
        isMoving = false;
    
        // 根据当前位置显示或隐藏左右按钮
        prevBtn.gameObject.SetActive(currentIndex > 0);
        nextBtn.gameObject.SetActive(currentIndex < fixedPositions.Length - 1);
    }
}
