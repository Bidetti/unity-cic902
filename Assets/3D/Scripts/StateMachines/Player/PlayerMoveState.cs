using UnityEngine; 

public class PlayerMoveState : PlayerBaseState{
    public PlayerMoveState(PlayerStateMachine stateMachine) : base(stateMachine) { }

    public override void Tick(float deltaTime){
        Vector3 movement = CalculateMovement(); 

        stateMachine.Controller.Move(movement * deltaTime * stateMachine.MovementSpeed); 

        if (stateMachine.InputReader.MovementValue == Vector2.zero){
            stateMachine.Animator.SetFloat("Blend", 0, 0.1f, deltaTime); 
            return; 
        }

        stateMachine.Animator.SetFloat("Blend", 1, 0.1f, deltaTime); 
        stateMachine.transform.rotation = Quaternion.LookRotation(movement); 
    }

    public override void Enter()
    { 
        stateMachine.InputReader.JumpEvent    += OnJump;
        stateMachine.InputReader.AttackEvent  += OnAttack;
        stateMachine.InputReader.TargetEvent  += OnTarget;
    }

    public override void Exit()
    {
        stateMachine.InputReader.JumpEvent    -= OnJump;
        stateMachine.InputReader.AttackEvent  -= OnAttack;
        stateMachine.InputReader.TargetEvent  -= OnTarget;
    }
    
    private void OnJump()
    {
        stateMachine.SwitchState(new PlayerJumpState(stateMachine));
    }

    private void OnAttack()
    {
        // inicia o combo no golpe 1
        stateMachine.SwitchState(new PlayerAttackState(stateMachine, 1));
    }

    private void OnTarget()
    {
        stateMachine.SwitchState(new PlayerTargetingState(stateMachine));
    }

    private Vector3 CalculateMovement()
    {
        Vector3 foward = stateMachine.MainCameraTransform.forward;
        Vector3 right = stateMachine.MainCameraTransform.right;

        foward.y = 0f;
        right.y = 0f;
        foward.Normalize();
        right.Normalize();

        return foward * stateMachine.InputReader.MovementValue.y + right * stateMachine.InputReader.MovementValue.x;
    }
}
