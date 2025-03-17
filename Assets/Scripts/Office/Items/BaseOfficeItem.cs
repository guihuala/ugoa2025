using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BaseOfficeItem : MonoBehaviour
{
    private Vector3 originalScale = new Vector3(1, 1, 1);
    private PlayerInteractManager playerInteractManager;

    protected virtual void Start()
    {
        HideHighlight();
        playerInteractManager = FindObjectOfType<PlayerInteractManager>();
    }

    private void Update()
    {
        if (!playerInteractManager.CheckItem(this))
        {
            return;
        }

        CheckHover();
        CheckClick();
    }

    private void CheckHover()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            // 如果射线命中了物体
            if (hit.collider.gameObject == gameObject)
            {
                ShowHighlight(1.3f); // 鼠标悬停到物体上时高亮
            }
            else
            {
                ShowHighlight(1.1f); // 放大1.1倍
            }
        }
    }

    public void ShowHighlight(float scaleFactor)
    {
        // 确保物体被激活并且不重复执行缩放
        if (playerInteractManager.CheckItem(this) && transform.localScale != originalScale * scaleFactor)
        {
            transform.DOScale(originalScale * scaleFactor, 0.3f)
                .SetEase(Ease.OutBack)
                .SetUpdate(true);
        }
    }

    public void HideHighlight()
    {
        transform.DOScale(new Vector3(1,1,1), 0.3f)
            .SetEase(Ease.InBack)
            .SetUpdate(true);
    }

    private void CheckClick()
    {
        if (Input.GetMouseButtonDown(0)) // 左键点击
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // 确保点击的物体是当前激活的物体
                if (hit.collider.gameObject == gameObject)
                {
                    Apply();
                }
            }
        }
    }

    protected virtual void Apply() 
    {
        // 应用逻辑
    }
}
