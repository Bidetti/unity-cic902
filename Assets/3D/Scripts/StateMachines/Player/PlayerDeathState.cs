using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        stateMachine.Animator.SetTrigger("Death");
        stateMachine.Controller.enabled = false;
        stateMachine.InputReader.enabled = false;
        Debug.Log("Game Over – jogador morreu");
        Time.timeScale = 0f;
    }

    public override void Tick(float deltaTime) { }
    public override void Exit() { }
}
