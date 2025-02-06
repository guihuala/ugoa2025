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

    private IEnumerator MoveAlongPath()
    {
        isMoving = true;

        while (pathQueue.Count > 0)
        {
            yield return new WaitForSeconds(followDelay + 1.2f);
            
            if (Time.timeScale == 0)
            {
                yield return new WaitUntil(() => Time.timeScale > 0);
            }

            Vector3 targetPosition = pathQueue.Dequeue();

            HandleRotation(targetPosition - transform.position);

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
            EVENTMGR.TriggerPlayerDead();
        }
    }
}
