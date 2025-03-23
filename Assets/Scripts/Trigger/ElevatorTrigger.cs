using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ElevatorTrigger : MonoBehaviour, IEnterSpecialItem
{
    private Vector3 originalPosition;  // 记录物体的初始位置
    public Vector3 targetPosition;     // 目标下降位置
    public float moveSpeed = 2f;       // 物体移动的速度
    
    private bool isMovingDown = false;  // 判断物体是否已经下降
    
    private Transform playerTransform;  // 玩家对象的位置
    private PlayerMovement playerMovement;
    
    private void Start()
    {
        originalPosition = transform.position;
        
        playerTransform = FindObjectOfType<Player>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
    }

    public void Apply()
    {
        if (isMovingDown)
        {
            StartCoroutine(MoveToPosition(originalPosition));
        }
        else
        {
            StartCoroutine(MoveToPosition(targetPosition));
        }
        
        isMovingDown = !isMovingDown;
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        Vector3 startPlayerPosition = playerTransform.position; // 玩家初始位置
        float journeyLength = Vector3.Distance(startPosition, target);
        float startTime = Time.time;

        // 平滑移动电梯和玩家，直到到达目标位置
        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;

            // 移动电梯
            transform.position = Vector3.Lerp(startPosition, target, fractionOfJourney);

            // 同步移动玩家
            playerTransform.position = Vector3.Lerp(startPlayerPosition, target + playerMovement.PositionOffset , fractionOfJourney);

            yield return null;
        }

        // 确保电梯和玩家精确到达目标位置
        transform.position = target;
        playerTransform.position = target + playerMovement.PositionOffset;
    }
}
