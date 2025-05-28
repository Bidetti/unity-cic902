using UnityEngine;

public class BossAttackState : BossBaseState
{
    private float elapsed;
    private float duration;
    private bool  hasDamaged;

    // dados do impulso
    private Vector3 impulseDir;
    private float   impulseSpeed;
    private const float impulseDuration = 0.2f; // duração do dash em segundos

    public BossAttackState(BossStateMachine sm) : base(sm) { }

    public override void Enter()
    {
        elapsed     = 0f;
        hasDamaged  = false;

        int idx       = sm.currentAttackIndex;
        float impulse = sm.attackImpulses[idx];

        // calcula direção e velocidade do impulso
        impulseDir   = sm.transform.forward;
        impulseSpeed = impulse / impulseDuration;

        // para o NavMeshAgent para passarmos a controlar manualmente
        sm.Agent.isStopped = true;

        // dispara animação de ataque correta
        if      (idx == 0) sm.Animator.SetTrigger("Punch");
        else if (idx == 1) sm.Animator.SetTrigger("Swipe");
        else               sm.Animator.SetTrigger("JumpAttack");

        // define duração do ataque (ajuste conforme duração real da animação)
        duration = (idx == 2 ? 1.5f : 1.0f);

        // prepara o próximo índice no loop
        sm.currentAttackIndex = (idx + 1) % sm.attackImpulses.Length;
    }

    public override void Tick(float deltaTime)
    {
        elapsed += deltaTime;

        // 1) durante a janela de impulso, movemos gradualmente
        if (elapsed <= impulseDuration)
        {
            // agent.Move aplica movimento respeitando o NavMesh
            sm.Agent.Move(impulseDir * impulseSpeed * deltaTime);
        }

        // 2) na metade da animação, aplica dano uma única vez
        if (!hasDamaged && elapsed >= duration * 0.5f)
        {
            hasDamaged = true;
            if (sm.PlayerHealth != null &&
                Vector3.Distance(sm.transform.position, sm.player.position) <= sm.attackRange)
            {
                sm.PlayerHealth.TakeDamage(sm.attackDamage);
            }
        }

        // 3) ao fim do ataque, inicia cooldown e volta ao Idle
        if (elapsed >= duration)
        {
            sm.nextAttackTime = Time.time + sm.attackCooldown;
            sm.SwitchState(new BossIdleState(sm));
        }
    }

    public override void Exit()
    {
        // nada extra aqui — o IdleState reativa o agent quando entrar
    }
}
