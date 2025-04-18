using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Enemy1 : MonoBehaviour
{
    public float moveSpeed = 3f;
    public int baseDamage = 10;
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 2f;
    public float attackCooldown = 2f;
    private float attackTimer;
    public GameObject damageTextPrefab;
    public GameObject enemyHealthBarPrefab;
    public BoxCollider2D attackCollider;
    public float attackRange = 1.5f;
    public float attackHeight = 1.0f;

    private int maxHealth;
    private int currentHealth;
    private Transform player;
    private Player playerScript;
    private bool isChasing = false;
    public bool isTutorialEnemy = false;

    private Slider enemyHealthBar; // Declaração do campo para corrigir CS0103

    private Animator animator;

    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath;

    void Start()
    {
        maxHealth = Random.Range(250, 751);
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();
        attackTimer = attackCooldown;

        if (attackCollider == null)
        {
            attackCollider = gameObject.AddComponent<BoxCollider2D>();
            attackCollider.isTrigger = true;
            attackCollider.size = new Vector2(attackRange, attackHeight);
            attackCollider.offset = new Vector2(attackRange / 2, 0);
            attackCollider.enabled = false;
        }

        InstantiateHealthBar();
    }

    void Update()
    {
        if (isChasing && player != null)
        {
            float distance = Vector3.Distance(transform.position, player.position);

            if (distance > attackRange)
            {
                animator.SetBool("isRunning", true);
                Vector3 direction = (player.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                if (direction.x != 0)
                {
                    transform.localScale = new Vector3(Mathf.Sign(-direction.x) * Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
                }
            }
            else
            {
                animator.SetBool("isRunning", false);

                if (attackTimer <= 0)
                {
                    PerformAttack();
                    attackTimer = attackCooldown;
                }
            }
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
        UpdateHealthBarPosition();
    }

    public void StartChasingPlayer()
    {
        isChasing = true;
    }

    void PerformAttack()
    {
        int damage = baseDamage;
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }

        animator.SetTrigger("attack1");
        playerScript.TakeDamage(damage, isCritical);

    }
    public void TakeDamage(int damage, bool isCritical = false)
    {
        currentHealth -= damage;
        animator.SetTrigger("hurt");
        GameObject healthBarObj = Instantiate(enemyHealthBarPrefab, transform.position, Quaternion.identity);

        if (enemyHealthBar != null)
        {
            enemyHealthBar.value = currentHealth;
        }
        else
        {
            Debug.LogError("Componente Slider não encontrado no prefab enemyHealthBar!");
        }
        Vector3 textPosition = transform.position + new Vector3(0f, 0.7f, 0f);

        // Adicione um pequeno offset aleatório horizontal para evitar sobreposição
        textPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0f, 0f);

        var damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);

        damageText textScript = damageTextObj.GetComponent<damageText>();
        if (textScript != null)
        {
            textScript.SetDamage(damage, isCritical, false);
        }
        else
        {
            damageTextObj.SendMessage("SetDamage", damage);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void InstantiateHealthBar()
    {
        enemyHealthBarPrefab = Instantiate(enemyHealthBarPrefab, transform.position + new Vector3(0, 1.2f, 0), Quaternion.identity);
        enemyHealthBarPrefab.transform.SetParent(transform);
        enemyHealthBarPrefab.transform.localPosition = new Vector3(0, 1.2f, 0);
        enemyHealthBar = enemyHealthBarPrefab.GetComponentInChildren<Slider>();
        if (enemyHealthBar != null)
        {
            enemyHealthBar.maxValue = maxHealth;
            enemyHealthBar.value = currentHealth;
        }
        else
        {
            Debug.LogError("Slider não encontrado no prefab enemyHealthBar!");
        }
    }

    void UpdateHealthBarPosition()
    {
        if (enemyHealthBarPrefab != null)
        {
            // Atualiza a posição da health bar para ficar sempre acima da cabeça do inimigo
            enemyHealthBarPrefab.transform.position = transform.position + new Vector3(0, 1.2f, 0);
        }
    }

    void Die()
    {
        animator.SetTrigger("death");

        if (OnDeath != null)
        {
            OnDeath.Invoke();
        }
        Destroy(gameObject);
    }
}
