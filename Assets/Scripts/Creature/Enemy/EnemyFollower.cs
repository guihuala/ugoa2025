using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Collision))]
public class EnemyFollower : MonoBehaviour
{
    private Queue<Vector3> pathQueue = new Queue<Vector3>();
    private bool isMoving = false;

    private float followDelay;
    
    private float currentRotation = 0f;
    private float targetRotation = 180f;
    
    [SerializeField]
    private Vector3 positionOffset = Vector3.zero;

    private EnemyBase targetEnemy;

    /// <summary>
    /// 设置跟随路径和延迟时间
    /// </summary>
    /// <param name="path">路径队列</param>
    /// <param name="delay">跟随延迟</param>
    public void FollowPath(Queue<Vector3> path, float delay)
    {
        pathQueue.Clear();
        foreach (var point in path)
        {
            pathQueue.Enqueue(point);
        }

        followDelay = delay;  // 设置小弟的跟随延迟

        if (!isMoving)
        {
            StartCoroutine(MoveAlongPath());
        }
    }

    public void SetTargetEnemy(EnemyBase enemy)
    {
        targetEnemy = enemy;
    }

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            // 每次移动前等待设定的延迟时间
            yield return new WaitForSeconds(followDelay + 1.2f);
            
            if (Time.timeScale == 0)
            {
                yield return new WaitUntil(() => Time.timeScale > 0);
            }

            // 从路径中取出目标点，并添加偏移量
            Vector3 targetPosition = pathQueue.Dequeue() + positionOffset;

            HandleRotation(targetPosition - transform.position);

            // 添加横向挤压弹性效果
            // 这里的 DOPunchScale 会让对象的局部缩放先向 X 方向收缩、Y 方向增大（实现横向挤压效果），然后弹回原状
            transform.DOPunchScale(new Vector3(-0.2f, 0.2f, 0f), 0.3f, 5, 0.5f);

            // 开始平滑移动到目标点
            while ((transform.position - targetPosition).sqrMagnitude > 0.01f)
            {
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, 3f * Time.deltaTime);
                yield return null;
            }
        }

        isMoving = false;
    }
    
    private void HandleRotation(Vector3 direction)
    {
        if (Mathf.Abs(direction.x) > 0.01f)
        {
            targetRotation = direction.x > 0 ? 180f : 0f;
            if (!Mathf.Approximately(targetRotation, currentRotation)) // 避免重复旋转
            {
                transform.DORotate(new Vector3(0f, targetRotation, 0f), 0.3f, RotateMode.FastBeyond360);
                currentRotation = targetRotation;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player>())
        {
            if(targetEnemy !=null)
                targetEnemy.PerformFoundPlayer();
        }
    }
}
