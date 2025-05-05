using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelSelectionCamera : MonoBehaviour
{
    [Header("相机基本配置")]
    public float shakeMagnitude = 0.05f;
    public float shakeFrequency = 1.0f;
    public float transitionTime = 0.5f;

    [Header("组件配置")]
    public Button prevBtn;
    public Button nextBtn;
    public Vector3[] fixedPositions;
    
    [Header("材质配置")]
    public Material skyboxMaterial;
    public Color startColor0;
    public Color startColor1;
    public Color[] targetColor0;
    public Color[] targetColor1;
    
    private float timeOffsetX;
    private float timeOffsetY;
    private int currentIndex = 1;
    private Vector3 lastMousePosition;
    private bool isDragging = false;
    private float dragThreshold = 50f;
    private Vector3 currentTargetPosition;
    private bool isMoving = false;

    // PlayerPrefs keys
    private const string CameraIndexKey = "LevelSelectionCamera_CurrentIndex";
    private const string SceneNameKey = "LevelSelectionCamera_SceneName";

    private void Awake()
    {
        string lastScene = PlayerPrefs.GetString(SceneNameKey, "");
        string currentScene = SceneManager.GetActiveScene().name;
        
        if (lastScene == currentScene)
        {
            currentIndex = PlayerPrefs.GetInt(CameraIndexKey, 1);
        }
        else
        {
            currentIndex = 1;
            PlayerPrefs.SetInt(CameraIndexKey, currentIndex);
        }
        
        PlayerPrefs.SetString(SceneNameKey, currentScene);
    }

    private void Start()
    {
        prevBtn.onClick.AddListener(MoveLeft);
        nextBtn.onClick.AddListener(MoveRight);
        
        timeOffsetX = Random.Range(0f, 100f);
        timeOffsetY = Random.Range(0f, 100f);

        currentTargetPosition = fixedPositions[currentIndex];
        Vector3 startPosition = fixedPositions[currentIndex]; // Start from saved position
        transform.position = startPosition;

        // Only animate if we're not at the default position
        if (currentIndex != 1)
        {
            StartCoroutine(SmoothMove(startPosition, currentTargetPosition, transitionTime));
        }
        else
        {
            // Immediately set colors for default position
            skyboxMaterial.SetColor("_Color0", targetColor0[currentIndex]);
            skyboxMaterial.SetColor("_Color1", targetColor1[currentIndex]);
            startColor0 = targetColor0[currentIndex];
            startColor1 = targetColor1[currentIndex];
            
            // Update button states
            UpdateButtonStates();
        }
    }

    private void Update()
    {
        if (isMoving) return; // 移动时不处理震动

        float xShake = Mathf.PerlinNoise(Time.time * shakeFrequency + timeOffsetX, 0) * 2f - 1f;
        float yShake = Mathf.PerlinNoise(0, Time.time * shakeFrequency + timeOffsetY) * 2f - 1f;
        xShake *= shakeMagnitude;
        yShake *= shakeMagnitude;

        transform.position = currentTargetPosition + new Vector3(xShake, yShake, 0f);

        HandleSwipe();
    }

    private void HandleSwipe()
    {
        if (isMoving) return;

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

    public void MoveLeft()
    {
        if (currentIndex > 0 && !isMoving)
        {
            currentIndex--;
            SaveCurrentPosition();
            MoveToPosition(currentIndex);
            prevBtn.interactable = false;
            nextBtn.interactable = false;
        }
    }

    private void MoveRight()
    {
        if (currentIndex < fixedPositions.Length - 1 && !isMoving)
        {
            currentIndex++;
            SaveCurrentPosition();
            MoveToPosition(currentIndex);
            prevBtn.interactable = false;
            nextBtn.interactable = false;
        }
    }

    private void MoveToPosition(int index)
    {
        if (isMoving) return;
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
            Vector3 newPos = Vector3.Lerp(startPos, endPos, t);

            Color newColor0 = Color.Lerp(startColor0, targetColor0[currentIndex], t);
            Color newColor1 = Color.Lerp(startColor1, targetColor1[currentIndex], t);
            skyboxMaterial.SetColor("_Color0", newColor0);
            skyboxMaterial.SetColor("_Color1", newColor1);
            
            transform.position = newPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    
        transform.position = endPos;
        skyboxMaterial.SetColor("_Color0", targetColor0[currentIndex]);
        skyboxMaterial.SetColor("_Color1", targetColor1[currentIndex]);
        startColor0 = targetColor0[currentIndex];
        startColor1 = targetColor1[currentIndex];
    
        currentTargetPosition = endPos;
        isMoving = false;
    
        UpdateButtonStates();
    }

    private void SaveCurrentPosition()
    {
        PlayerPrefs.SetInt(CameraIndexKey, currentIndex);
        PlayerPrefs.Save();
    }

    private void UpdateButtonStates()
    {
        prevBtn.interactable = true;
        nextBtn.interactable = true;
        prevBtn.gameObject.SetActive(currentIndex > 0);
        nextBtn.gameObject.SetActive(currentIndex < fixedPositions.Length - 1);
    }

    private void OnDestroy()
    {
        PlayerPrefs.Save();
    }
}