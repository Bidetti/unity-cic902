using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private int   comboIndex;
    private float elapsed, duration;
    private bool  queued;

    public PlayerAttackState(PlayerStateMachine stateMachine, int comboIndex) : base(stateMachine)
    {
        this.comboIndex = comboIndex;
    }

    public override void Enter()
    {
        // duração aproximada de cada animação (ajuste conforme clipe)
        duration = comboIndex < 3 ? 0.7f : 1f;
        elapsed  = 0f;
        queued   = false;

        // dispara trigger Attack1, Attack2 ou Attack3
        stateMachine.Animator.SetTrigger("Attack" + comboIndex);

        // inscrever para enfileirar o próximo golpe
        stateMachine.InputReader.AttackEvent += QueueNext;

        // causar dano se boss estiver no alcance
        if (stateMachine.BossHealth != null)
        {
            float dist = Vector3.Distance(
                stateMachine.transform.position,
                stateMachine.BossHealth.transform.position);
            if (dist <= stateMachine.AttackRange)
                stateMachine.BossHealth.TakeDamage(stateMachine.AttackDamage);
        }
    }

    public override void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        if (elapsed < duration) return;

        // ao terminar animação:
        stateMachine.InputReader.AttackEvent -= QueueNext;
        if (queued && comboIndex < 3)
            stateMachine.SwitchState(new PlayerAttackState(stateMachine, comboIndex + 1));
        else
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }

    public override void Exit()
    {
        // já removemos o handler acima
    }

    private void QueueNext() => queued = true;
}
