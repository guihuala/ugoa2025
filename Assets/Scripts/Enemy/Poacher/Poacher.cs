using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Poacher : EnemyBase
{
    private int currentPointIndex = 0;
    

    protected override void InitializeStates()
    {
        stateMachine.ChangeState(new PatrolState(this));
    }

    public override void MoveForward()
    {
        if (patrolPoints.Length == 0) return;
        
        Vector3 targetPosition = patrolPoints[currentPointIndex].position;
        Vector3 direction = (targetPosition - transform.position).normalized;

        // 确保方向不是零向量
        if (direction != Vector3.zero)
        {
            if (CheckPoint != null)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                CheckPoint.rotation = Quaternion.Slerp(CheckPoint.rotation, targetRotation, moveSpeed * Time.deltaTime);
            }
            
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        // 如果接近目标点，则切换到下一个巡逻点
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }


    public void StartWobblingHead()
    {
        // 头部摇摆的动画
    }

    public void StopWobblingHead()
    {
        // 停止头部摇摆的动画
    }

    public override void PerformFoundPlayer()
    {
        // 触发失败事件
        EVENTMGR.TriggerPlayerDead();
    }
}