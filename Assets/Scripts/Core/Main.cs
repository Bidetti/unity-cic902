using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    public AudioClip menuMusic;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = menuMusic;
        audioSource.loop = true;
        audioSource.volume = 0.6f;
        audioSource.Play();
        Debug.Log("Tocando música do menu");
    }

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