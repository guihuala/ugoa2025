using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Animator playerAnimator;

    private float horizontalInput;
    private float verticalInput;

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        
        Vector3 moveDirection = new Vector3(horizontalInput, 0f, verticalInput);
        moveDirection = moveDirection.normalized;
        
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        
        FlipBasedOnDirection(horizontalInput);
        
        HandleAnimations(moveDirection);
    }

    private void FlipBasedOnDirection(float horizontal)
    {
        if (horizontal > 0)
        {
            playerSprite.flipX = false;
        }
        else if (horizontal < 0)
        {
            playerSprite.flipX = true;
        }
    }

    private void HandleAnimations(Vector3 moveDirection)
    {
        if (playerAnimator != null)
        {
            playerAnimator.SetFloat("Speed", moveDirection.magnitude);
        }
    }
}