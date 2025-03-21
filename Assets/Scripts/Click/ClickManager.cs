using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private ClickableEffect currentClickableEffect; // 当前激活的物体
    
    private bool isActive = true;

    [Header("Camera Settings")]
    public bool useOrthographicCamera = true; // 默认使用正交相机

    private OrthographicCameraController _orthographicCameraController;    
    private PerspectiveCameraController _perspectiveCameraController;

    private CameraController _cameraController;  // 通用相机控制器
    
    private void Awake()
    {
        // 根据是否使用正交相机，选择相机控制器
        if (useOrthographicCamera)
        {
            _orthographicCameraController = FindObjectOfType<OrthographicCameraController>();
            _cameraController = _orthographicCameraController;
        }
        else
        {
            _perspectiveCameraController = FindObjectOfType<PerspectiveCameraController>();
            _cameraController = _perspectiveCameraController;
        }

        if (_cameraController == null)
        {
            Debug.LogError("CameraController not found in the scene!");
        }
    }

    private void Start()
    {
        EVENTMGR.OnClickPlayer += HandleClickPlayer;
        EVENTMGR.OnTimeScaleChange += HandleTimeScaleChange;
        EVENTMGR.OnClickPath += CloseEffect;

        EVENTMGR.OnPlayerDead += PlayerDead;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnClickPlayer -= HandleClickPlayer;
        EVENTMGR.OnTimeScaleChange -= HandleTimeScaleChange;
        EVENTMGR.OnClickPath -= CloseEffect;
        
        EVENTMGR.OnPlayerDead -= PlayerDead;
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
        if (Time.timeScale == 0)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            IClickable clickable = hit.collider.GetComponent<IClickable>();
            if (clickable != null)
            {
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
        }
    }
    
    private void PlayerDead()
    {
        isActive = false;
        
        if(currentClickableEffect != null)
            CloseEffect();
    }

    private void HandleClickPlayer(bool isActivity)
    {
        if (currentClickableEffect == null) return;
        
        if (!isActive)
            return;

        if (isActivity)
        {
            currentClickableEffect.ShowUIWithAnimation();
        }
        else
        {
            currentClickableEffect.HideUIWithAnimation();
            currentClickableEffect = null;
        }
    }

    private void HandleTimeScaleChange(float newTimeScale)
    {
        Time.timeScale = newTimeScale;
    }
}
