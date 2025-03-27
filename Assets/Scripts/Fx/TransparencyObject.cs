using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyObject : MonoBehaviour
{
    private Material originalMaterial;
    public Material transparentMaterial;

    private void Start()
    {
        originalMaterial = GetComponent<MeshRenderer>().material;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            OnBecameInvisible();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        OnBecameVisible();
    }

    public void OnBecameVisible()
    {
        // 恢复原材质
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = originalMaterial;
        }
    }

    public void OnBecameInvisible()
    {
        // 更改材质为半透明
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.material = transparentMaterial;
        }
    }
}