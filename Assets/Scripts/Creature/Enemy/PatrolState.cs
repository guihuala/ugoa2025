using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : IState
{
    private EnemyBase enemy;

    public PatrolState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        Debug.Log($"{enemy.name} 进入侦查状态");
    }

    public void Execute()
    {
        // 如果玩家在侦查范围内
        if (enemy.IsPlayerDetected())
        {
            enemy.ChangeState(new FailState(enemy));
        }
        else
        {
            enemy.MoveForward();
        }
    }

    public void Exit()
    {
        Debug.Log($"{enemy.name} 退出侦查状态");
    }
}