using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoUI : MonoBehaviour
{
    public Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => gameObject.SetActive(false));
    }
}
