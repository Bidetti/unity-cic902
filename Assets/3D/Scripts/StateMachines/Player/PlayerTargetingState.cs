using UnityEngine;

public class PlayerTargetingState : PlayerBaseState
{
    // Velocidade de giro para mirar no boss
    private const float rotationSpeed = 5f;

    public PlayerTargetingState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // escuta ESC para sair do modo targeting
        stateMachine.InputReader.CancelTargetEvent += OnCancel;
    }

    public override void Tick(float deltaTime)
    {
        // gira o jogador suavemente para olhar na direção do boss
        if (stateMachine.BossHealth != null)
        {
            Transform bossT = stateMachine.BossHealth.transform;
            Vector3 dir = bossT.position - stateMachine.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.01f)
            {
                Quaternion targetRot = Quaternion.LookRotation(dir);
                stateMachine.transform.rotation = Quaternion.Slerp(
                    stateMachine.transform.rotation,
                    targetRot,
                    rotationSpeed * deltaTime
                );
            }
        }
        // (aqui você poderia incluir lógica de “atacar enquanto mira”, 
        // se quiser responder a AttackEvent também)
    }

    public override void Exit()
    {
        stateMachine.InputReader.CancelTargetEvent -= OnCancel;
    }

    private void OnCancel()
    {
        // volta ao estado de movimento livre
        stateMachine.SwitchState(new PlayerMoveState(stateMachine));
    }
}
