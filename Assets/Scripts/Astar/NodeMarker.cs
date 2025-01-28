using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeMarker : MonoBehaviour
{
    public bool IsWalkable = true; // 是否可行走

    private SpriteRenderer clickHighLight;
    private SpriteRenderer walkableHighLight;
    public bool IsHighlighted { get; private set; } = false;
    
    void Start()
    {
        clickHighLight = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (clickHighLight == null)
        {
            Debug.LogError("ClickHighLight is null");
        }

        walkableHighLight = transform.GetChild(1).GetComponent<SpriteRenderer>();
        if (walkableHighLight == null)
        {
            Debug.LogError("walkableHighLight is null");
        }

        clickHighLight.gameObject.SetActive(false);
        HideHighlight();
    }

    void OnMouseEnter()
    {
        if (IsWalkable && IsHighlighted && clickHighLight != null)
        {
            clickHighLight.gameObject.SetActive(true);
        }
    }

    void OnMouseExit()
    {
        if (clickHighLight != null)
        {
            clickHighLight.gameObject.SetActive(false);
        }
    }


    public void ShowHighlight()
    {
        if (walkableHighLight != null && IsWalkable)
        {
            walkableHighLight.gameObject.SetActive(true);
            IsHighlighted = true;
        }
    }

    public void HideHighlight()
    {
        if (walkableHighLight != null)
        {
            walkableHighLight.gameObject.SetActive(false);
            IsHighlighted = false;
        }
    }
}