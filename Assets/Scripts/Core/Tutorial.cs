using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class Tutorial : MonoBehaviour
{
    public TextMeshProUGUI tutorialText;
    public TextMeshProUGUI subtitleText;
    public Player player;
    public GameManager gameManager;
    public GameObject enemyPrefab;
    public Transform spawnPoint;
    public AudioClip victoryMusic;
    public AudioClip battleMusic;
    private AudioSource audioSource;
    public int currentStep = 0;
    private bool enemyKilled = false;
    private bool itemPickedUp = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        ShowNextStep();
    }

    void Update()
    {
        if (currentStep == 5 && enemyKilled)
        {
            CompleteTutorial();
        }
    }

    void ShowNextStep()
    {
        switch (currentStep)
        {
            case 0:
                tutorialText.text = "1. Movimentos";
                subtitleText.text = "Use AD para se mover.";
                break;
            case 1:
                tutorialText.text = "2. Pulo";
                subtitleText.text = "Pressione W para pular.";
                break;
            case 2:
                tutorialText.text = "3. Ataque Basico";
                subtitleText.text = "Pressione SPACE para atacar. (Delay: 0.5f)";
                break;
            case 3:
                tutorialText.text = "4. Pressione 1 para o segundo ataque especial.";
                subtitleText.text = "Este ataque tem um alcance maior. (Delay: 5s)";
                break;
            case 4:
                tutorialText.text = "5. Pressione 2 para o terceiro ataque especial.";
                subtitleText.text = "Este ataque causa dano em area. (Delay: 7.5s)";
                break;
            case 5:
                tutorialText.text = "6. Desafio";
                subtitleText.text = "Mate o inimigo";
                SpawnEnemy();
                PlayBattleMusic();
                break;
                //case 7:
                //    tutorialText.text = "Pegue o item dropado pelo inimigo.";
                //    subtitleText.text = "";
                //    break;
        }
    }

    public void OnPlayerAction()
    {
        //if (currentStep == 7 && !itemPickedUp)
        //{
        //    return;
        //}
        if (currentStep == 5 && !enemyKilled)
        {
            return;
        }
        currentStep++;
        ShowNextStep();
    }

    void SpawnEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        Enemy1 enemyScript = enemy.GetComponent<Enemy1>();
        enemyScript.OnDeath += OnEnemyKilled;
        enemyScript.isTutorialEnemy = true;
    }

    public void OnEnemyKilled()
    {
        enemyKilled = true;
        Debug.Log("Inimigo morto!");
    }

    public void OnItemPickedUp()
    {
        itemPickedUp = true;
    }

    void CompleteTutorial()
    {
        tutorialText.text = "Tutorial concluido! Parabens!";
        subtitleText.text = "Em 15 segundos voce sera redirecionado para a tela inicial.";
        PlayVictoryMusic();
        StartCoroutine(WaitAndLoadMainScene());
    }

    void PlayBattleMusic()
    {
        if (battleMusic != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = battleMusic;
            audioSource.loop = true;
            audioSource.volume = 0.6f;
            audioSource.Play();
            Debug.Log("Tocando música de batalha");
        }
        else
        {
            Debug.LogWarning("Música de batalha não configurada!");
        }
    }

    void PlayVictoryMusic()
    {
        if (victoryMusic != null && audioSource != null)
        {
            audioSource.Stop();
            audioSource.clip = victoryMusic;
            audioSource.loop = false;
            audioSource.volume = 0.7f;
            audioSource.Play();
            Debug.Log("Tocando música de vitória");
        }
        else
        {
            Debug.LogWarning("Música de vitória não configurada!");
        }
    }

    IEnumerator WaitAndLoadMainScene()
    {
        yield return new WaitForSeconds(15);
        SceneManager.LoadScene("Main");
    }
}