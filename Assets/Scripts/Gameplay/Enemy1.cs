using UnityEngine;
using UnityEngine.UI;

public class Enemy1 : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int baseDamage = 10;
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 2f;

    public GameObject[] itemPrefabs;
    public bool isTutorialEnemy = false;
    public Transform damageTextPosition;

    private int maxHealth;
    private int currentHealth;
    private float[] attackCooldowns = { 5f, 7.5f, 15f };
    private float[] attackTimers = { 0f, 0f, 0f };

    private Transform player;
    private Player playerScript;
    private bool isChasing = false;

    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath;

    void Start()
    {
        maxHealth = Random.Range(250, 751);
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
    }

    void Update()
    {
        if (isChasing)
        {
            Move();
            HandleAttacks();
        }
        UpdateAttackTimers();
    }

    public void StartChasingPlayer()
    {
        isChasing = true;
    }

    void Move()
    {
        if (player != null)
        {
            Vector3 direction = (player.position - transform.position).normalized;
            transform.position += direction * moveSpeed * Time.deltaTime;
        }
    }

    void HandleAttacks()
    {
        if (player != null && Vector3.Distance(transform.position, player.position) < 1.5f)
        {
            if (attackTimers[0] <= 0) SpecialAttack(1, 0);
            if (attackTimers[1] <= 0) SpecialAttack(2, 1);
            if (attackTimers[2] <= 0) SpecialAttack(3, 2);
        }
    }

    void Attack(int attackType)
    {
        int damage = baseDamage * attackType;
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }
        playerScript.TakeDamage(damage);
    }

    void SpecialAttack(int attackType, int index)
    {
        Attack(attackType);
        attackTimers[index] = attackCooldowns[index];
    }

    void UpdateAttackTimers()
    {
        for (int i = 0; i < attackTimers.Length; i++)
        {
            if (attackTimers[i] > 0)
            {
                attackTimers[i] -= Time.deltaTime;
            }
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        ShowDamage(damage);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void ShowDamage(int damage)
    {
        GameObject damageText = Instantiate(playerScript.damageTextPrefab, damageTextPosition.position, Quaternion.identity);
        damageText.GetComponent<Text>().text = damage.ToString();
        Destroy(damageText, 1f);
    }

    void Die()
    {
        if (OnDeath != null)
        {
            OnDeath();
        }
        DropItem();
        Destroy(gameObject);
    }

    void DropItem()
    {
        float dropChance = isTutorialEnemy ? 1f : Mathf.Clamp01(maxHealth / 1000f);
        if (Random.value < dropChance)
        {
            GameObject itemPrefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(itemPrefab, transform.position, Quaternion.identity);
        }
    }
}