using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    public Text tutorialText;
    public Text subtitleText; // Novo campo para o subtítulo
    public Player player;
    public GameManager gameManager;
    public GameObject enemyPrefab;
    public Transform spawnPoint;

    private int currentStep = 0;
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
            ShowNextStep();
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
                tutorialText.text = "Use WASD para se mover.";
                subtitleText.text = "";
                break;
            case 1:
                tutorialText.text = "Pressione W para pular.";
                subtitleText.text = "";
                break;
            case 2:
                tutorialText.text = "Pressione Espaço para atacar.";
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
                subtitleText.text = "Este ataque causa dano em área.";
                break;
            case 6:
                tutorialText.text = "Pegue um item do chão.";
                subtitleText.text = "";
                break;
            case 7:
                tutorialText.text = "Mate o inimigo para concluir o tutorial.";
                subtitleText.text = "";
                SpawnEnemy();
                break;
            case 8:
                tutorialText.text = "Pegue o item dropado pelo inimigo.";
                subtitleText.text = "";
                break;
        }
    }

    public void OnPlayerAction()
    {
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
    }
}