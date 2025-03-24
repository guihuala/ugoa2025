using System;
using System.Collections;
using UnityEngine;

public class MovingTrigger : MonoBehaviour, IEnterSpecialItem, IExitSpecialItem
{
    public Vector3[] movePoints;
    public float moveSpeed = 1f;  // 物体移动的速度
    
    private Transform playerTransform;  // 玩家对象的位置
    private PlayerMovement playerMovement;
    private int currentPointIndex = 0;  // 当前目标点索引
    private bool isPlayerFollowing = false;  // 玩家是否应该跟随物体

    private void Start()
    {
        playerTransform = FindObjectOfType<Player>().transform;
        playerMovement = FindObjectOfType<PlayerMovement>();
        
        // 初始化路径
        if (movePoints.Length > 0)
        {
            StartCoroutine(MoveAlongPath());
        }
    }
    
    public void Apply()
    {
        isPlayerFollowing = true; // 当玩家站在物体上时，允许玩家跟随
    }

    public void Exit()
    {
        isPlayerFollowing = false;
    }

    private IEnumerator MoveAlongPath()
    {
        while (true)
        {
            Vector3 targetPosition = movePoints[currentPointIndex];
            yield return StartCoroutine(MoveToPosition(targetPosition)); // 移动到目标位置

            // 移动到下一个路径点，循环回来
            currentPointIndex = (currentPointIndex + 1) % movePoints.Length;

            // 控制移动间隔
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator MoveToPosition(Vector3 target)
    {
        Vector3 startPosition = transform.position;
        Vector3 startPlayerPosition = playerTransform.position;  // 玩家初始位置
        float journeyLength = Vector3.Distance(startPosition, target);
        float startTime = Time.time;

        while (Vector3.Distance(transform.position, target) > 0.01f)
        {
            float distanceCovered = (Time.time - startTime) * moveSpeed;
            float fractionOfJourney = distanceCovered / journeyLength;
            float easedT = Mathf.SmoothStep(0f, 1f, fractionOfJourney); // 先快后慢曲线

            // 移动物体
            transform.position = Vector3.Lerp(startPosition, target, easedT);

            // 如果允许玩家跟随，则同步移动玩家
            if (isPlayerFollowing)
            {
                playerTransform.position = Vector3.Lerp(startPlayerPosition, target + playerMovement.PositionOffset, easedT);
            }

            yield return null; // 等待下一帧
        }

        // 确保物体和玩家精确到达目标位置
        transform.position = target;
        if (isPlayerFollowing)
        {
            playerTransform.position = target + playerMovement.PositionOffset;
        }
    }
}
