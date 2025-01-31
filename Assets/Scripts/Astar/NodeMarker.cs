using UnityEngine;
using DG.Tweening;

public class NodeMarker : MonoBehaviour
{
    public bool IsWalkable = true;
    
    private SpriteRenderer clickHighLight;
    private GameObject hoverHighlight;
    public bool IsHighlighted { get; private set; } = false;

    private Vector3 originalScale = new Vector3(1, 1, 1);

    void Start()
    {
        clickHighLight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        hoverHighlight = transform.GetChild(1).gameObject;

        if (clickHighLight == null) Debug.LogError("ClickHighLight is null");

        clickHighLight.gameObject.SetActive(false);
        hoverHighlight.SetActive(false);
        
        HideHighlight();
    }

    void OnMouseEnter()
    {
        if (IsWalkable && IsHighlighted && clickHighLight != null)
        {
            clickHighLight.gameObject.SetActive(true);
            clickHighLight.DOFade(1f, 0.3f).SetUpdate(true); // 渐显，忽略时间缩放

            transform.DOScale(new Vector3(originalScale.x, originalScale.y * 1.2f, originalScale.z), 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
    }

    void OnMouseExit()
    {
        if (clickHighLight != null)
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
}
