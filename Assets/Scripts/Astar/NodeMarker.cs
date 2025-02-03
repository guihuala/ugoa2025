using UnityEngine;
using DG.Tweening;

public class NodeMarker : MonoBehaviour
{
    [Header("能否走过")] public bool IsWalkable = true;

    [Header("脚印消失时间")] [SerializeField] private float footprintDuration = 2.0f;

    private SpriteRenderer clickHighLight;
    private GameObject hoverHighlight;
    private GameObject footPrint;
    public bool IsHighlighted { get; private set; } = false;

    private Vector3 originalScale = new Vector3(1, 1, 1);
    
    private bool isFootPrintPlaying = false;
    private Tween footPrintTween;

    void Start()
    {
        clickHighLight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        hoverHighlight = transform.GetChild(1).gameObject;
        footPrint = transform.GetChild(2).gameObject;

        if (clickHighLight == null) Debug.LogError("ClickHighLight is null");

        clickHighLight.gameObject.SetActive(false);
        hoverHighlight.SetActive(false);
        footPrint.SetActive(false);

        HideHighlight();
    }

    void Update()
    {
        if(!IsWalkable)
            return;
        
        CheckHover();
        CheckClick();
    }

    void CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject && IsHighlighted && clickHighLight != null)
            {
                // 鼠标悬停到物体上，显示高亮效果
                if (!clickHighLight.gameObject.activeSelf)
                {
                    clickHighLight.gameObject.SetActive(true);
                    clickHighLight.DOFade(1f, 0.3f).SetUpdate(true);

                    transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.2f, originalScale.z), 0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
            }
            else
            {
                // 鼠标离开物体，恢复状态
                if (clickHighLight.gameObject.activeSelf)
                {
                    clickHighLight.DOFade(0f, 0.3f)
                        .SetUpdate(true)
                        .OnComplete(() => clickHighLight.gameObject.SetActive(false));

                    if (!IsHighlighted)
                    {
                        transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.3f)
                            .SetEase(Ease.InBack)
                            .SetUpdate(true);
                    }
                    else
                    {
                        transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.1f, originalScale.z), 0.3f)
                            .SetEase(Ease.OutBack)
                            .SetUpdate(true);
                    }
                }
            }
        }
        else
        {
            // 没有射线击中物体时
            if (clickHighLight.gameObject.activeSelf)
            {
                clickHighLight.DOFade(0f, 0.3f)
                    .SetUpdate(true)
                    .OnComplete(() => clickHighLight.gameObject.SetActive(false));

                if (!IsHighlighted)
                {
                    transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.3f)
                        .SetEase(Ease.InBack)
                        .SetUpdate(true);
                }
                else
                {
                    transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.1f, originalScale.z), 0.3f)
                        .SetEase(Ease.OutBack)
                        .SetUpdate(true);
                }
            }
        }
    }
    
    void CheckClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    if (!IsHighlighted)
                    {
                        EVENTMGR.TriggerClickPath();
                        return;
                    }

                    EVENTMGR.TriggerClickMarker(transform.position);
                }
            }
        }
    }

    public void ShowHighlight()
    {
        if (IsWalkable)
        {
            transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.1f, originalScale.z), 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);

            IsHighlighted = true;

            hoverHighlight.SetActive(true);
        }
    }

    public void HideHighlight()
    {
        transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);

        IsHighlighted = false;

        hoverHighlight.SetActive(false);
    }
    
    public void ShowFootPrint()
    {
        if (footPrint != null)
        {
            if (isFootPrintPlaying && footPrintTween != null)
            {
                footPrintTween.Kill();
            }

            footPrint.SetActive(true);

            // 重置脚印的透明度
            footPrint.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            // 启动脚印渐隐动画，并且重置脚印动画
            footPrintTween = footPrint.GetComponent<SpriteRenderer>().DOFade(0f, footprintDuration)
                .OnComplete(() =>
                {
                    footPrint.SetActive(false);
                    isFootPrintPlaying = false;
                });

            isFootPrintPlaying = true;
        }
    }
}
