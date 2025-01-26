using System;
using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private float rayDistance = 1f;

    private void Update()
    {
        DetectWithRaycast();
    }

    private void DetectWithRaycast()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, rayDistance))
        {
            var item = hit.collider.GetComponent<IEnterSpecialItem>();
            if (item != null)
                item.Apply();
        }
    }
}