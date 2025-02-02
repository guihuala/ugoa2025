using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FailState : IState
{
    private EnemyBase enemy;

    public FailState(EnemyBase enemy)
    {
        this.enemy = enemy;
    }

    public void Enter()
    {
        enemy.PerformFoundPlayer();
    }

    public void Execute() { }
    public void Exit() { }
}
