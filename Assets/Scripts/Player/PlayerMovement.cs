using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;  // 引入 DOTween 命名空间

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;

    private float horizontalInput;
    private float verticalInput;

    private float currentRotation = 0f;
    private float targetRotation = 180f;

    void Start()
    {
        transform.rotation = Quaternion.Euler(0f, currentRotation, 0f);
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection = moveDirection.normalized;

        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);

        HandleRotation(horizontalInput);
        HandleAnimations(moveDirection);
    }

    private void HandleRotation(float horizontal)
    {
        if (horizontal > 0)
        {
            targetRotation = 180f;
        }
        else if (horizontal < 0)
        {
            targetRotation = 0f;
        }

        // 平滑过渡旋转
        if (Mathf.Abs(targetRotation - currentRotation) > 0.1f) // 防止频繁触发过渡
        {
            transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360); // 使用 DOTween 来平滑旋转
            currentRotation = targetRotation; // 更新当前旋转角度
        }
    }


    private void HandleAnimations(Vector3 moveDirection)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", moveDirection.magnitude); // 设置动画参数控制行走
        }
    }
}
