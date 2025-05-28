using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    private float verticalVelocity;

    public PlayerJumpState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        // Inicia o pulo com força configurada
        verticalVelocity = stateMachine.JumpForce;
        stateMachine.Animator.SetTrigger("Jump");

        // Pequeno deslocamento pra garantir que o CharacterController "solte" do chão
        stateMachine.Controller.Move(Vector3.up * 0.1f);
    }

    public override void Tick(float deltaTime)
    {
        // --- Movimentação horizontal ---
        Vector3 horizontalDir = CalculateMovement();
        Vector3 horizontalVel = horizontalDir * stateMachine.MovementSpeed;

        // --- Gravidade ---
        verticalVelocity += Physics.gravity.y * deltaTime;

        // --- Aplica movimento total ---
        Vector3 totalVelocity = horizontalVel + Vector3.up * verticalVelocity;
        stateMachine.Controller.Move(totalVelocity * deltaTime);

        // --- Rotaciona para a direção do input, se houver ---
        if (stateMachine.InputReader.MovementValue != Vector2.zero)
        {
            Vector3 lookDir = new Vector3(horizontalVel.x, 0f, horizontalVel.z);
            stateMachine.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        // --- Chegou ao chão de novo? volta ao estado de movimentação ---
        if (stateMachine.Controller.isGrounded && verticalVelocity < 0f)
        {
            stateMachine.SwitchState(new PlayerMoveState(stateMachine));
        }
    }

    public override void Exit()
    {
        // nada a limpar
    }

    // Reúne o cálculo de direção relativa à câmera (mesma lógica do PlayerMoveState)
    private Vector3 CalculateMovement()
    {
        Vector3 forward = stateMachine.MainCameraTransform.forward;
        Vector3 right   = stateMachine.MainCameraTransform.right;
        forward.y = 0f;
        right.y   = 0f;
        forward.Normalize();
        right.Normalize();

        Vector2 input = stateMachine.InputReader.MovementValue;
        return forward * input.y + right * input.x;
    }
}
