using UnityEngine;

public class SnailSkillState : BaseState
{
    private Character character;
    public override void OnEnter(Enemy enemy)
    {
        currentEnemy = enemy;
        currentEnemy.currentSpeed = currentEnemy.chaseSpeed;
        currentEnemy.animator.SetBool("hide", true);
        currentEnemy.animator.SetTrigger("skill");
        if (!character) character = currentEnemy.GetComponent<Character>();
        character.invulnerable = true;
        currentEnemy.lostTimeCounter = currentEnemy.lostTime;
        character.invulnerableCounter = currentEnemy.lostTimeCounter;
    }

    public override void LogicUpdate()
    {
        if (currentEnemy.lostTimeCounter <= 0) currentEnemy.SwitchState(currentEnemy.patrolState);
        character.invulnerableCounter = currentEnemy.lostTimeCounter;
    }

    public override void PhysicsUpdate()
    {

    }

    public override void OnExit()
    {
        currentEnemy.animator.SetBool("hide", false);
        character.invulnerable = false;
    }
}
