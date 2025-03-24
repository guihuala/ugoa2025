using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunState : IState
{
    private EnemyBase enemy;
    private float stunDuration = 10f; // 眩晕持续时间
    private float timer = 0f;

    public StunState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.stopMoving = true; // 禁止敌人移动
        timer = 0f; // 重置计时器
    }

    public void Execute()
    {
        timer += Time.deltaTime;

        if (timer >= stunDuration)
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit()
    {
        enemy.stopMoving = false; // 恢复敌人移动
    }
}
