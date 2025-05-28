using UnityEngine;

public class BossIdleState : BossBaseState
{
    public BossIdleState(BossStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        sm.Agent.isStopped    = true;
        sm.Animator.SetBool("isWalking", false);
        sm.Animator.SetBool("isRunning", false);
    }

    public override void Tick(float dt)
    {
        if (sm.player == null) return;

        float dist = Vector3.Distance(sm.transform.position, sm.player.position);
        bool inSight    = dist <= sm.chaseRange;
        bool offCooldown = Time.time >= sm.nextAttackTime;

        if (inSight)
        {
            if (dist <= sm.attackRange && offCooldown)
            {
                sm.SwitchState(new BossAttackState(sm));
            }
            else
            {
                sm.SwitchState(new BossChaseState(sm));
            }
        }
    }

    public override void Exit() { }
}
 