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

    public int currentStep = 0;
    private bool enemyKilled = false;
    private bool itemPickedUp = false;

    void Start()
    {
        ShowNextStep();
    }

    void Update()
    {
        if (currentStep == 8 && enemyKilled)
        {
            OnPlayerAction();
        }
        if (currentStep == 9 && itemPickedUp)
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

    void OnEnemyKilled()
    {
        enemyKilled = true;
    }

    public void OnItemPickedUp()
    {
        itemPickedUp = true;
    }

    void CompleteTutorial()
    {
        tutorialText.text = "Tutorial concluído!";
        subtitleText.text = "Clique E no portal para continuar.";
        StartCoroutine(WaitAndLoadMainScene());
    }

    IEnumerator WaitAndLoadMainScene()
    {
        // Tocar música aqui
        // AudioSource audioSource = GetComponent<AudioSource>();
        // audioSource.Play();

        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Main");
    }
}