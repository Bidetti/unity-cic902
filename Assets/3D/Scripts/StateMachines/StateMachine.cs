using UnityEngine;

public class StateMachine : MonoBehaviour{
    private State currentState;

    // Método responsável por trocar o estado atual da máquina de estados
    public void SwitchState(State newState){
        // Se houver um estado atual, chama o método Exit() para finalizar
        currentState?.Exit();

        // Define o novo estado como o estado atual
        currentState = newState;

        // Chama o método Enter() do novo estado para inicializar sua lógica
        currentState?.Enter();
    }

    public void Update(){
        currentState?.Tick(Time.deltaTime);
    }
}
