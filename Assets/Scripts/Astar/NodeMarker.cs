using UnityEngine;
using DG.Tweening;

public class NodeMarker : MonoBehaviour
{
    public bool IsWalkable = true;
    
    private SpriteRenderer clickHighLight;
    private MeshRenderer meshRenderer;
    public bool IsHighlighted { get; private set; } = false;

    private Vector3 originalScale = new Vector3(1, 1, 1);
    
    public Material highlightMaterial; // 高亮材质
    private Material originalMaterial; // 原始材质

    void Start()
    {
        clickHighLight = transform.GetChild(0).GetComponent<SpriteRenderer>();

        if (clickHighLight == null) Debug.LogError("ClickHighLight is null");

        clickHighLight.gameObject.SetActive(false);
        HideHighlight();
        
        meshRenderer = GetComponent<MeshRenderer>();
        if (meshRenderer != null && meshRenderer.materials.Length > 1)
        {
            originalMaterial = meshRenderer.materials[1];
        }
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
            
            if (meshRenderer != null && meshRenderer.materials.Length > 1)
            {
                Material[] mats = meshRenderer.materials;
                mats[1] = highlightMaterial;
                meshRenderer.materials = mats;
            }
        }
    }

    public void HideHighlight()
    {
        transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);

        IsHighlighted = false;
        
        if (meshRenderer != null && meshRenderer.materials.Length > 1)
        {
            Material[] mats = meshRenderer.materials;
            mats[1] = originalMaterial;
            meshRenderer.materials = mats;
        }
    }
}
