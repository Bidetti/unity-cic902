using UnityEngine;

public class BossHurtState : BossBaseState
{
    private float elapsed;
    private const float hurtTime = 1f;

    public BossHurtState(BossStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        elapsed = 0f;
        sm.Agent.isStopped = true;
        sm.Animator.SetTrigger("Hurt");
    }

    public override void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        if (elapsed >= hurtTime)
        {
            sm.SwitchState(new BossIdleState(sm));
        }
    }

    public override void Exit() { }
}
