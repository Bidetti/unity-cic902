using UnityEngine;

public class BossChaseState : BossBaseState
{
    public BossChaseState(BossStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        sm.Agent.isStopped = false;
    }

    public override void Tick(float dt)
    {
        if (sm.player == null) return;

        float dist       = Vector3.Distance(sm.transform.position, sm.player.position);
        bool offCooldown = Time.time >= sm.nextAttackTime;

        // escolhe se caminha ou corre
        if (offCooldown)
        {
            sm.Agent.speed = sm.runSpeed;
            sm.Animator.SetBool("isRunning", true);
            sm.Animator.SetBool("isWalking", false);
        }
        else
        {
            sm.Agent.speed = sm.walkSpeed;
            sm.Animator.SetBool("isWalking", true);
            sm.Animator.SetBool("isRunning", false);
        }

        // direciona o agente para o jogador
        sm.Agent.SetDestination(sm.player.position);

        // transições de estado
        if (dist <= sm.attackRange && offCooldown)
        {
            sm.SwitchState(new BossAttackState(sm));
        }
        else if (dist > sm.chaseRange)
        {
            // se o jogador fugir do chaseRange, volta ao Idle
            sm.SwitchState(new BossIdleState(sm));
        }
    }

    public override void Exit()
    {
        sm.Agent.isStopped = true;
        sm.Animator.SetBool("isWalking", false);
        sm.Animator.SetBool("isRunning", false);
    }
}
