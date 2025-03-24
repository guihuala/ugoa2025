using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundState : IState
{
    private EnemyBase enemy;
    private float enterTime;
    private float exitDelay = 2f;

    public FoundState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.stopMoving = true; // 禁止敌人移动
        enterTime = Time.time;
    }

    public void Execute()
    {
        // 如果玩家在侦查范围内
        if (enemy.IsPlayerDetected())
        {
            enemy.UpdateBar((Time.time - enterTime) / exitDelay);
            
            if (Time.time - enterTime >= exitDelay)
            {
                enemy.PerformAttackPlayer();
            }
        }
        else
        {
            enemy.UpdateBar(-1f);
            enemy.stopMoving = false; // 恢复敌人移动
            enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit()
    {
        
    }
}
