using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;
    private ClickableEffect currentClickableEffect; // 当前激活的物体
    [SerializeField] private float sizeLerpSpeed = 15f; // 相机缩放速度
    [SerializeField] private float targetSize = 2.5f; // 缩放目标正交大小
    
    private float normalSize; // 记录默认相机大小
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
        if (IsPointerOverUI()) return;
        if (Time.timeScale == 0)
            return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
                // if (currentClickableEffect != null && currentClickableEffect != hit.collider.GetComponent<ClickableEffect>())
                // {
                //     CloseEffect();
                //     return;
                // }

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
