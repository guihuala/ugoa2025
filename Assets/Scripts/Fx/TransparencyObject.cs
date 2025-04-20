using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparencyObject : MonoBehaviour
{
    private Material[] originalMaterials;
    public Material transparentMaterial;

    private void Start()
    {
        originalMaterials = GetComponent<MeshRenderer>().materials;
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
        // 存储原始材质
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            renderer.materials = originalMaterials;
        }
    }

    public void OnBecameInvisible()
    {
        // 所有材质设置为透明材质
        Renderer renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
        {
            Material[] transparentMaterials = new Material[originalMaterials.Length];
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                transparentMaterials[i] = transparentMaterial;
            }
            renderer.materials = transparentMaterials;
        }
    }
}
