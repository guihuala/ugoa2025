using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseOfficeItem : MonoBehaviour
{
    private Vector3 originalScale = new Vector3(1, 1, 1);

    public bool IsHighlighted { get; private set; } = false;
    public bool IsActive { get; set; } = false;

    protected virtual void Start()
    {
        HideHighlight();
    }

    protected virtual void Update()
    {
        // 检查鼠标悬停和点击
        CheckHover();
        CheckClick();
    }

    private void CheckHover()
    {
        if(!IsActive)
            HideHighlight();
        
        RaycastHit hit;
        if (RaycastToObject(out hit))
        {
            // 如果射线命中了物体
            if (!IsHighlighted)
            {
                IsHighlighted = true;
                ShowHighlight(1.3f);  // 鼠标悬停到物体上时高亮
            }
        }
        else
        {
            // 如果射线没有命中物体
            if (IsHighlighted)
            {
                IsHighlighted = false;
                ShowHighlight(1.1f);  // 鼠标离开物体
            }
        }
    }

    private void CheckClick()
    {
        if(!IsActive)
            return;
        
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            RaycastHit hit;
            if (RaycastToObject(out hit))
            {
                Apply();
            }
        }
    }
    
    private bool RaycastToObject(out RaycastHit hit)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == gameObject;
    }

    protected virtual void Apply() 
    {
        // 应用逻辑
        Debug.Log("Item applied!");
    }

    public void ShowHighlight(float scaleFactor)
    {
        transform.DOScale(originalScale * scaleFactor, 0.3f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true);
    }

    public void HideHighlight()
    {
        transform.DOScale(new Vector3(originalScale.x, originalScale.y, originalScale.z), 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }
}
