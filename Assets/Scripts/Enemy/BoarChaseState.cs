using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoarChaseState : BaseState
{
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.animator.SetBool("run", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.physicsCheck.touchFrontWall || !currentEnemy.physicsCheck.isGround)
            currentEnemy.transform.localScale = Vector3.Scale(currentEnemy.transform.localScale, new Vector3(-1, 1, 1));
        if (currentEnemy.lostTimeCounter <= 0) currentEnemy.SwitchState(currentEnemy.patrolState);
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("run", false);
    }
}
