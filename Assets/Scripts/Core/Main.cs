using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    // Método para iniciar o jogo
    public void IniciarJogo()
    {
        SceneManager.LoadScene("gamePhase01");
    }

    // Método para abrir o tutorial
    public void AbrirTutorial()
    {
        SceneManager.LoadScene("gameTutorial");
    }

    // Método para sair do jogo
    public void SairJogo()
    {
        Application.Quit();
    }
}