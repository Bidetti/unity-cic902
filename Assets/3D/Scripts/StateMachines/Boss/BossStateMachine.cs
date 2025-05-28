using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Health))]
public class BossStateMachine : StateMachine
{
    [Header("Parâmetros")]
    public float   chaseRange    = 20f;
    public float   attackRange   = 2.5f;
    public float   attackCooldown= 5f;
    public int     attackDamage  = 15;
    public float walkSpeed = 2f;
    public float runSpeed  = 5f;
    public float[] attackImpulses = new float[3] { 1f, 1.5f, 2f };

    [HideInInspector] public float nextAttackTime;
    [HideInInspector] public int   currentAttackIndex;

    [HideInInspector] public Transform player;
    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public Animator      Animator;
    [HideInInspector] public Health        Health;
    [HideInInspector] public Health        PlayerHealth;

    private void Start()
    {
        Agent            = GetComponent<NavMeshAgent>();
        Animator         = GetComponent<Animator>();
        Health           = GetComponent<Health>();
        nextAttackTime   = Time.time;
        currentAttackIndex = 0;

        player = GameObject.FindWithTag("Player")?.transform;
        PlayerHealth = player?.GetComponent<Health>();

        Health.OnHurt += () => SwitchState(new BossHurtState(this));
        Health.OnDeath += () => SwitchState(new BossDeathState(this));
        SwitchState(new BossIdleState(this));
    }
}
