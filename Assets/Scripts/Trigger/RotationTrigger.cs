using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTrigger : MonoBehaviour, IEnterSpecialItem
{
    public float rotationSpeed = 360;
    public float targetRotation;
    
    private PerspectiveCameraController perspectiveCameraController;
    private PlayerMovement playerMovement;
    private bool isRotating = false;
    private bool isAtTarget = false;

    private void Start()
    {
        perspectiveCameraController = FindObjectOfType<PerspectiveCameraController>();
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void Apply()
    {
        if (isRotating) return;
        
        float targetAngle = targetRotation;
        StartCoroutine(RotateCamera(targetAngle));
    }

    private IEnumerator RotateCamera(float targetAngle)
    {
        isRotating = true;
        
        while (Mathf.Abs(Mathf.DeltaAngle(perspectiveCameraController.angle_y, targetAngle)) > 0.1f)
        {
            perspectiveCameraController.angle_y = Mathf.MoveTowardsAngle(
                perspectiveCameraController.angle_y,
                targetAngle,
                rotationSpeed * Time.deltaTime);
                
            yield return null;
        }
        
        perspectiveCameraController.angle_y = targetAngle;
        isAtTarget = !isAtTarget;
        
        playerMovement.ChangeRotation(targetAngle + 180);
        
        isRotating = false;
    }
}