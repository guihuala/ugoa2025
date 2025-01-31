using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;
    private ClickableEffect currentClickableEffect; // 当前激活的物体
    [SerializeField] private float sizeLerpSpeed = 15f; // 相机缩放速度
    [SerializeField] private float targetSize = 2f; // 缩放目标正交大小
    private float normalSize; // 默认正交大小
    private Coroutine sizeChangeCoroutine;

    private void Awake()
    {
        mainCamera = Camera.main;

        if (mainCamera == null)
        {
            Debug.LogError("Main Camera not found! Please ensure your camera has the 'MainCamera' tag.");
        }
    }

    private void Start()
    {
        EVENTMGR.OnClickPlayer += HandleClickPlayer;
        EVENTMGR.OnTimeScaleChange += HandleTimeScaleChange;
        EVENTMGR.OnClickPath += CloseEffect;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnClickPlayer -= HandleClickPlayer;
        EVENTMGR.OnTimeScaleChange -= HandleTimeScaleChange;
        EVENTMGR.OnClickPath -= CloseEffect;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            DetectClick();
        }
    }
    
    private void DetectClick()
    {
        // 如果鼠标点在 UI 上，则不执行 3D 点击逻辑
        if (IsPointerOverUI()) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 检测物体是否实现 IClickable 接口
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                // 如果已经有激活的物体先关闭
                if (currentClickableEffect != null && currentClickableEffect != hit.collider.GetComponent<ClickableEffect>())
                {
                    currentClickableEffect.HideUIWithAnimation();
                    EVENTMGR.TriggerClickPlayer(false);
                }

                // 激活新的物体
                currentClickableEffect = hit.collider.GetComponent<ClickableEffect>();
                if (currentClickableEffect != null)
                {
                    clickable.OnClick();
                }
            }
        }
        else
        {
            CloseEffect();
        }
    }

    private bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
    
        return results.Count > 0; // 如果射线检测到 UI，则返回 true
    }


    private void CloseEffect()
    {
        if (currentClickableEffect != null)
        {
            currentClickableEffect.HideUIWithAnimation();
            EVENTMGR.TriggerClickPlayer(false);
            EVENTMGR.TriggerTimeScaleChange(1.0f);
            currentClickableEffect = null;

            // 恢复相机缩放
            StartCameraZoom(normalSize);
        }
    }

    private void HandleClickPlayer(bool isActivity)
    {
        if (currentClickableEffect == null) return;

        if (isActivity)
        {
            currentClickableEffect.ShowUIWithAnimation();
            
            StartCameraZoom(targetSize);
        }
        else
        {
            currentClickableEffect.HideUIWithAnimation();
            currentClickableEffect = null;

            // 恢复相机缩放
            StartCameraZoom(normalSize);
        }
    }

    private void HandleTimeScaleChange(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }

    private void StartCameraZoom(float targetSize)
    {
        normalSize = mainCamera.orthographicSize;
        
        if (sizeChangeCoroutine != null)
        {
            StopCoroutine(sizeChangeCoroutine);
        }
        sizeChangeCoroutine = StartCoroutine(ChangeCameraSize(targetSize));
    }

    private IEnumerator ChangeCameraSize(float targetSize)
    {
        while (Mathf.Abs(mainCamera.orthographicSize - targetSize) > 0.01f)
        {
            mainCamera.orthographicSize = Mathf.Lerp(mainCamera.orthographicSize, targetSize, Time.deltaTime * sizeLerpSpeed);
            yield return null;
        }

        mainCamera.orthographicSize = targetSize;
    }
}
