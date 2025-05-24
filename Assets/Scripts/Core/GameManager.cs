using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public Player player;
    public int playerLevel = 1;
    public int healthIncreasePerLevel = 100;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI finalText;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject portalPrefab;
    public Transform portalSpawnPoint;
    public Transform[] spawnPoints;
    public AudioClip gameplayMusic;
    public AudioSource audioSource;

    private int enemiesAlive = 0;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = gameplayMusic;
        audioSource.loop = true;
        audioSource.volume = 0.3f;
        audioSource.Play();
        Debug.Log("Tocando música de batalha");

        levelText.text = "Level: " + playerLevel;
        player.IncreaseMaxHealth(0);
        GenerateEnemies();
        Player playerScript = player.GetComponent<Player>();
        playerScript.OnDeath += HandlePlayerDeath;
    }

    public IEnumerator LevelUp()
    {
        playerLevel++;
        player.IncreaseMaxHealth(healthIncreasePerLevel);
        LevelUpUI.Instance.ShowLevelUp(playerLevel);
        levelText.text = "Level: " + playerLevel;

        if (playerLevel < 5)
        {
            yield return new WaitForSeconds(3f); // <- Espera aqui antes de gerar inimigos

            GenerateEnemies();
        }
        else
        {
            finalText.gameObject.SetActive(true);
            SpawnPortal();
        }
    }

    void GenerateEnemies()
    {
        if (player.controlMode == Player.ControlMode.Tutorial)
        {
            Debug.Log("Modo tutorial ativo. Nenhum inimigo será gerado.");
            return;
        }

        ClearExistingEnemies();
        int numberOfEnemies = playerLevel;
        enemiesAlive = 0;
        
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyPrefab = GetEnemyPrefab();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            GameObject enemyObj = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);

            IEnemy enemy = enemyObj.GetComponent<IEnemy>();
            if (enemy is Enemy1 enemy1)
            {
                enemy1.OnDeath += OnEnemyDeath;
            }
            else if (enemy is Enemy2 enemy2)
            {
                enemy2.OnDeath += OnEnemyDeath;
            }

            enemiesAlive++;
        }

        
    }

    GameObject GetEnemyPrefab()
    {
        float randomValue = Random.value;
        if (randomValue < 0.7f)
        {
            return enemy1Prefab;
        }
        else
        {
            return enemy2Prefab;
        }
    }

    void ClearExistingEnemies()
    {
        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemy in existingEnemies)
        {
            Destroy(enemy);
        }
    }

    void HandlePlayerDeath()
    {
        Debug.Log("Player morreu! Reiniciando o jogo...");
        playerLevel = 1;
        StartCoroutine(WaitAndLoadMainScene());
    }

    private void OnEnemyDeath()
    {
        enemiesAlive--;

        if (enemiesAlive <= 0)
        {
            StartCoroutine(LevelUp());
        }
    }

    private void SpawnPortal()
    {
        Instantiate(portalPrefab, portalSpawnPoint.position, portalSpawnPoint.rotation);
    }

    IEnumerator WaitAndLoadMainScene()
    {
        yield return new WaitForSeconds(15);
        SceneManager.LoadScene("Main");
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }
}