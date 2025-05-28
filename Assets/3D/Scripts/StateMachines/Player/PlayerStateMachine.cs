using UnityEngine;

public class PlayerStateMachine : StateMachine{
    [field: SerializeField] public float MovementSpeed  { get; private set; } = 3f;
    [field: SerializeField] public float JumpForce      { get; private set; } = 2.5f;
    [field: SerializeField] public float AttackRange    { get; private set; } = 1f;
    [field: SerializeField] public int   AttackDamage   { get; private set; } = 10;

    public Transform            MainCameraTransform { get; private set; }
    public InputReader          InputReader         { get; private set; }
    public Animator             Animator            { get; private set; }
    public CharacterController  Controller          { get; private set; }
    public Health               Health              { get; private set; }
    public Health               BossHealth          { get; private set; }

    public void Start(){
        MainCameraTransform = Camera.main.transform;
        InputReader   = GetComponent<InputReader>();
        Animator      = GetComponent<Animator>();
        Controller    = GetComponent<CharacterController>();
        Health        = GetComponent<Health>();

        // referencia ao boss para causar dano
        var bossObj = GameObject.FindWithTag("Boss");
        if (bossObj != null) 
            BossHealth = bossObj.GetComponent<Health>();

        Health.OnHurt += () => SwitchState(new PlayerHurtState(this));

        // vai pro estado de morte quando morrer
        Health.OnDeath += () => SwitchState(new PlayerDeathState(this));

        // estado inicial: mover
        SwitchState(new PlayerMoveState(this));
    }
}
