using UnityEngine;
using DG.Tweening; // 引入 DOTween

public class NodeMarker : MonoBehaviour
{
    public bool IsWalkable = true; // 是否可行走

    private SpriteRenderer clickHighLight;
    private SpriteRenderer walkableHighLight;
    public bool IsHighlighted { get; private set; } = false;
    private Vector3 originalScale = new Vector3(1, 1, 1); // 记录原始大小

    void Start()
    {
        clickHighLight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        walkableHighLight = transform.GetChild(1).GetComponent<SpriteRenderer>();

        if (clickHighLight == null) Debug.LogError("ClickHighLight is null");
        if (walkableHighLight == null) Debug.LogError("walkableHighLight is null");

        clickHighLight.gameObject.SetActive(false);
        HideHighlight();
    }

    void OnMouseEnter()
    {
        if (IsWalkable && IsHighlighted && clickHighLight != null)
        {
            clickHighLight.gameObject.SetActive(true);
            clickHighLight.DOFade(1f, 0.3f); // 渐显
            transform.DOScale(originalScale * 1.2f, 0.3f).SetEase(Ease.OutBack); // 放大动画
        }
    }

    void OnMouseExit()
    {
        if (clickHighLight != null)
        {
            clickHighLight.DOFade(0f, 0.3f).OnComplete(() => clickHighLight.gameObject.SetActive(false)); // 渐隐
            transform.DOScale(originalScale, 0.3f).SetEase(Ease.InBack); // 缩小回原大小
        }
    }

    public void ShowHighlight()
    {
        if (walkableHighLight != null && IsWalkable)
        {
            walkableHighLight.gameObject.SetActive(true);
            walkableHighLight.DOFade(1f, 0.3f); // 渐显
            transform.DOScale(originalScale * 1.1f, 0.3f).SetEase(Ease.OutBack); // 放大
            IsHighlighted = true;
        }
    }

    public void HideHighlight()
    {
        if (walkableHighLight != null)
        {
            walkableHighLight.DOFade(0f, 0.3f).OnComplete(() => walkableHighLight.gameObject.SetActive(false)); // 渐隐
            transform.DOScale(originalScale, 0.3f).SetEase(Ease.InBack); // 缩小回原大小
            IsHighlighted = false;
        }
    }
}
