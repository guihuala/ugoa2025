using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FollowCameraWithOffset : MonoBehaviour
{
    [Header("相机引用")]
    [SerializeField] private Camera targetCamera;
    
    [Header("偏移设置")]
    [SerializeField] private Vector3 positionOffset = new Vector3(0, 0, 0);
    [SerializeField] private bool useLocalOffset = false;
    [SerializeField] private bool followX = true;
    [SerializeField] private bool followY = true;
    [SerializeField] private bool followZ = true;
    
    [Header("平滑")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private bool useSmoothing = true;

    private void Start()
    {
        if (targetCamera == null)
        {
            targetCamera = Camera.main;
        }
        
        UpdatePosition(instant: true);
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition(bool instant = false)
    {
        if (targetCamera == null)
            return;

        Vector3 targetPosition = CalculateTargetPosition();
        
        if (instant || !useSmoothing)
        {
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        }
    }

    private Vector3 CalculateTargetPosition()
    {
        Vector3 basePosition = targetCamera.transform.position;
        Vector3 offset = positionOffset;
        
        if (useLocalOffset)
        {
            offset = targetCamera.transform.TransformDirection(positionOffset);
        }
        
        Vector3 targetPosition = basePosition + offset;
        
        if (!followX) targetPosition.x = transform.position.x;
        if (!followY) targetPosition.y = transform.position.y;
        if (!followZ) targetPosition.z = transform.position.z;

        return targetPosition;
    }
}