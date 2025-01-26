using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SingleBtn : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Text _interactText;

    public string openPanelName;

    private void Awake()
    {
        _interactText = transform.GetChild(0).GetComponent<Text>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySfx("click");
        UIManager.Instance.OpenPanel(openPanelName);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _interactText.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _interactText.gameObject.SetActive(false);
    }
}