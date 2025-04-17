using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public Player player;
    public int playerLevel = 1;
    public int healthIncreasePerLevel = 100;
    public LevelUpUI levelUpUI;
    public TextMeshProUGUI levelText;
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;
    public GameObject bossPrefab;

    public Transform[] spawnPoints;
    public Transform bossSpawnPoint;

    void Start()
    {
        levelText.text = "Level: " + playerLevel;
        player.IncreaseMaxHealth(0);
        GenerateEnemies();
    }

    public void LevelUp()
    {
        playerLevel++;
        player.IncreaseMaxHealth(healthIncreasePerLevel);
        levelUpUI.ShowLevelUp(playerLevel);
        levelText.text = "Level: " + playerLevel;
        GenerateEnemies();
    }

    void GenerateEnemies()
    {
        if (player.controlMode == Player.ControlMode.Tutorial)
        {
            Debug.Log("Modo tutorial ativo. Nenhum inimigo será gerado.");
            return;
        }

        ClearExistingEnemies();

        int numberOfEnemies = playerLevel * 6;
        for (int i = 0; i < numberOfEnemies; i++)
        {
            GameObject enemyPrefab = GetEnemyPrefab();
            Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        }

        Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);
    }

    GameObject GetEnemyPrefab()
    {
        if (playerLevel < 5)
        {
            return enemy1Prefab;
        }
        else if (playerLevel < 10)
        {
            return enemy2Prefab;
        }
        else
        {
            return enemy3Prefab;
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
}