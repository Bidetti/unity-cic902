using UnityEngine;

public class PlayerHurtState : PlayerBaseState
{
    private float elapsed;
    private const float hurtTime = 1f;

    public PlayerHurtState(PlayerStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        elapsed = 0f;
        stateMachine.Animator.SetTrigger("Hurt");
        // não registra AttackEvent, logo não ataca neste estado
    }

    public override void Tick(float deltaTime)
    {
        elapsed += deltaTime;
        if (elapsed >= hurtTime)
        {
            // retorna ao movimento / idle após se recuperar
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }

    public override void Exit() { }
}
