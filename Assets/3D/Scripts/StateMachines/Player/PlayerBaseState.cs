using UnityEngine;

// Classe base abstrata para os estados do jogador (ex: correr, pular).
// Herda de 'State', que define os métodos Enter, Tick e Exit.
public abstract class PlayerBaseState : State{
    // Referência à máquina de estados do jogador.
    protected PlayerStateMachine stateMachine;

    // Construtor que recebe e armazena a máquina de estados.
    public PlayerBaseState(PlayerStateMachine stateMachine)
    {
        this.stateMachine = stateMachine;
    }
}
