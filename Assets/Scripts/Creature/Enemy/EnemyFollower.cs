using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class EnemyFollower : MonoBehaviour, IShootable
{
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;
    private bool stopMoving = false;
    private bool isStunned = false; // 是否处于眩晕状态

    private float followDelay;
    private float currentRotation = 0f;
    private float targetRotation = 180f;
    
    [SerializeField]
    private Vector3 positionOffset = Vector3.zero;

    private EnemyBase targetEnemy;
    private Coroutine moveCoroutine;
    private Coroutine stunCoroutine;

    /// <summary>
    /// 设置跟随路径和延迟时间
    /// </summary>
    public void FollowPath(Queue<Vector3> path, float delay)
    {
        if (isStunned) return; // 眩晕状态下不接收新路径

        pathQueue.Clear();
        foreach (var point in path)
        {
            pathQueue.Enqueue(point);
        }

        followDelay = delay;

        if (!isMoving && !stopMoving && moveCoroutine == null)
        {
            moveCoroutine = StartCoroutine(MoveAlongPath());
        }
    }

    public void SetTargetEnemy(EnemyBase enemy)
    {
        targetEnemy = enemy;
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0 && !isStunned) // 检查眩晕状态
        {
            yield return new WaitForSeconds(followDelay + 1.2f);
            
            if (Time.timeScale == 0)
            {
                yield return new WaitUntil(() => Time.timeScale > 0);
            }

            Vector3 targetPosition = pathQueue.Dequeue() + positionOffset;

            HandleRotation(targetPosition - transform.position);
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0f), 0.3f, 5, 0.5f);

            while ((transform.position - targetPosition).sqrMagnitude > 0.01f && !isStunned)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
        moveCoroutine = null;
    }
    
    private void HandleRotation(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            targetRotation = direction.x > 0 ? 180f : 0f;
            if (!Mathf.Approximately(targetRotation, currentRotation))
            {
                transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360);
                currentRotation = targetRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 眩晕状态下不与玩家交互
        if (isStunned) return;

        if (other.GetComponent<Player>())
        {
            if (targetEnemy != null)
            {
                targetEnemy.PerformAttackPlayer();
            }
        }
    }
    
    public void OnShot(BulletLifecycle bullet)
    {
        if (isStunned) return; // 已经处于眩晕状态则不再处理

        EVENTMGR.TriggerPlayerFound(); // 屏幕震动
        
        // 停止当前移动
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
        
        // 开始眩晕状态
        stunCoroutine = StartCoroutine(StunEffect());
    }

    private IEnumerator StunEffect()
    {
        isStunned = true;
        
        yield return new WaitForSeconds(3f); // 眩晕3秒
        
        // 恢复原状
        GetComponent<Renderer>().material.color = Color.white;
        isStunned = false;
        
        // 如果还有路径点，继续移动
        if (pathQueue.Count > 0 && !stopMoving)
        {
            moveCoroutine = StartCoroutine(MoveAlongPath());
        }
        
        stunCoroutine = null;
    }

    private void OnDestroy()
    {
        // 清理协程
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
    }
}