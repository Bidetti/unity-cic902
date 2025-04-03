using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{

    // M�todo para iniciar o jogo
    public void IniciarJogo()
    {
        SceneManager.LoadScene("gamePhase01");
    }

    // M�todo para abrir o tutorial
    public void AbrirTutorial()
    {
        SceneManager.LoadScene("gameTutorial");
    }

    // M�todo para sair do jogo
    public void SairJogo()
    {
        Application.Quit();
    }
}