using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SaveBtn : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => UIManager.Instance.OpenPanel("SavePanel"));
    }
}
