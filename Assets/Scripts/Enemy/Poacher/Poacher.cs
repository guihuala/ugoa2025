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

        // 获取目标点
        Vector3 targetPosition = patrolPoints[currentPointIndex].position;
        Vector3 direction = (targetPosition - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // 平滑旋转
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);

        // 移动到目标点（需要修改 不然某些方向上会变成纸片
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        
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
        Debug.Log("发现玩家");
        // 触发失败事件
    }
}