using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarPatrolState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.normalSpeed;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.physicsCheck.touchFrontWall || !currentEnemy.physicsCheck.isGround)
        {
            currentEnemy.wait = true;
            currentEnemy.animator.SetBool("walk", false);
        }
        else currentEnemy.animator.SetBool("walk", true);

        if (currentEnemy.FoundPlayer()) currentEnemy.SwitchState(currentEnemy.chaseState);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("walk", false);
        currentEnemy.wait = false;
        currentEnemy.waitTimeCounter = currentEnemy.waitTime;
    }
}
