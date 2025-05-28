public abstract class BossBaseState : State
{
    protected BossStateMachine sm;
    protected BossBaseState(BossStateMachine stateMachine) => sm = stateMachine;
    public override abstract void Enter();
    public override abstract void Tick(float deltaTime);
    public override abstract void Exit();
}
