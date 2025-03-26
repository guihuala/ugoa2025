using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MenuCameraController : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 5f; // 旋转速度

    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.up, rotationSpeed * Time.deltaTime);
    }
}