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
                tutorialText.text = "Use AD para se mover.";
                subtitleText.text = "";
                break;
            case 1:
                tutorialText.text = "Pressione W para pular.";
                subtitleText.text = "";
                break;
            case 2:
                tutorialText.text = "Pressione SPACE para atacar.";
                subtitleText.text = "";
                break;
            case 3:
                tutorialText.text = "Pressione 1 para o primeiro ataque especial.";
                subtitleText.text = "Este ataque causa dano extra.";
                break;
            case 4:
                tutorialText.text = "Pressione 2 para o segundo ataque especial.";
                subtitleText.text = "Este ataque tem um alcance maior.";
                break;
            case 5:
                tutorialText.text = "Pressione 3 para o terceiro ataque especial.";
                subtitleText.text = "Este ataque causa dano em area.";
                break;
            case 6:
                tutorialText.text = "Mate o inimigo";
                subtitleText.text = "";
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
        if (currentStep == 6 && !enemyKilled)
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
        subtitleText.text = "";
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