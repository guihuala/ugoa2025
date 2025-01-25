using System.Collections;
using UnityEngine;

public class ClickManager : MonoBehaviour
{
    private Camera mainCamera;
    private ClickableEffect currentClickableEffect; // 当前激活的物体
    [SerializeField] private float sizeLerpSpeed = 15f; // 相机缩放速度
    [SerializeField] private float targetSize = 2f; // 缩放目标正交大小
    [SerializeField] private float normalSize = 2.5f; // 默认正交大小
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
        EVENTMGR.OnClickCharacter += HandleClickCharacter;
        EVENTMGR.OnTimeScaleChange += HandleTimeScaleChange;
    }

    private void OnDestroy()
    {
        EVENTMGR.OnClickCharacter -= HandleClickCharacter;
        EVENTMGR.OnTimeScaleChange -= HandleTimeScaleChange;
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
                    EVENTMGR.TriggerClickCharacter(false);
                }

                // 激活新的物体
                currentClickableEffect = hit.collider.GetComponent<ClickableEffect>();
                if (currentClickableEffect != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }

    private void HandleClickCharacter(bool isActivity)
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
