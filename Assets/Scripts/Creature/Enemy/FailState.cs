using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailState : IState
{
    private EnemyBase enemy;
    private float enterTime;
    private float exitDelay = 2f;

    public FailState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.PerformFoundPlayer();
        enterTime = Time.time;
    }

    public void Execute()
    {
        if (Time.time - enterTime >= exitDelay)
        {
            enemy.ChangeState(new PatrolState(enemy));
        }
    }

    public void Exit() { }
}
