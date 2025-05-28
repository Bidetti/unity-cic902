using UnityEngine;

public class BossDeathState : BossBaseState
{
    public BossDeathState(BossStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        sm.Animator.SetTrigger("Death");
        sm.Agent.isStopped = true;
        Debug.Log("Boss derrotado! Vitória!");
        Time.timeScale = 0f;
    }

    public override void Tick(float dt) { }
    public override void Exit() { }
}
