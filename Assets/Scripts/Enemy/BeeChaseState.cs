using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeChaseState : BaseState
{
    private Attack attack;
    private Vector3 target;
    private Vector3 moveDir;
    private bool isAttack;
    private float attackRateCounter;

    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        attack = enemy.GetComponent<Attack>();
        currentEnemy.animator.SetBool("chase", true);
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0) currentEnemy.SwitchState(currentEnemy.patrolState);
        target = new Vector3(currentEnemy.attacker.position.x, currentEnemy.attacker.position.y + 1.5f);
        if (attackRateCounter > 0) attackRateCounter -= Time.deltaTime;

        if (Mathf.Abs(target.x - currentEnemy.transform.position.x) <= attack.attackRange && Mathf.Abs(target.y - currentEnemy.transform.position.y) <= attack.attackRange)
        {
            isAttack = true;
            if (!currentEnemy.isHurt) currentEnemy.rb.velocity = Vector2.zero;
            if (attackRateCounter <= 0)
            {
                currentEnemy.animator.SetTrigger("attack");
                attackRateCounter = attack.attackRange;
            }
        }
        else isAttack = false;


        moveDir = (target - currentEnemy.transform.position).normalized;
        if (moveDir.x > 0) currentEnemy.transform.localScale = new Vector3(-Mathf.Abs(currentEnemy.transform.localScale.x), currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
        else if (moveDir.x < 0) currentEnemy.transform.localScale = new Vector3(Mathf.Abs(currentEnemy.transform.localScale.x), currentEnemy.transform.localScale.y, currentEnemy.transform.localScale.z);
    }

    public override void PhysicsUpdate()
    {
        if (!currentEnemy.isHurt && !currentEnemy.isDead && !isAttack)
            currentEnemy.rb.velocity = moveDir * currentEnemy.currentSpeed * Time.deltaTime;
    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("chase", false);
    }
}
