using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SingleBtn : MonoBehaviour
{
    public string openPanelName;

    private Vector3 originalScale;
    private Vector3 originalPosition;

    [SerializeField] private float scaleFactor = 1.2f;
    [SerializeField] private float moveDistance = 0.1f;

    private bool isMouseOver = false;  // 用来判断鼠标是否在物体上方

    private void Awake()
    {
        originalScale = transform.localScale;
        originalPosition = transform.position;

        if (GetComponent<Collider>() == null)
        {
            gameObject.AddComponent<BoxCollider>();
        }
    }

    private void Update()
    {
        DetectMouseOver();
        
        if (isMouseOver && Input.GetMouseButtonDown(0))
        {
            OnPointerClick();
        }
    }
    
    private void DetectMouseOver()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (GetComponent<Collider>() != null && Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isMouseOver)
                {
                    OnMouseEnter();
                    isMouseOver = true;
                }
            }
            else
            {
                if (isMouseOver)
                {
                    OnMouseExit();
                    isMouseOver = false;
                }
            }
        }
        else
        {
            if (isMouseOver)
            {
                OnMouseExit();
                isMouseOver = false;
            }
        }
    }
    
    private void OnMouseEnter()
    {
        transform.DOScale(originalScale * scaleFactor, 0.2f).SetEase(Ease.OutBack); // 放大
        transform.DOMove(originalPosition + new Vector3(0, moveDistance, 0), 0.2f).SetEase(Ease.OutBack); // 上升
    }
    
    private void OnMouseExit()
    {
        transform.DOScale(originalScale, 0.2f).SetEase(Ease.InBack); // 恢复大小
        transform.DOMove(originalPosition, 0.2f).SetEase(Ease.InBack); // 恢复位置
    }

    // 点击事件
    public void OnPointerClick()
    {
        AudioManager.Instance.PlaySfx("click");
        UIManager.Instance.OpenPanel(openPanelName);
    }
}
