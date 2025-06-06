using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class Enemy1 : MonoBehaviour, IEnemy
{
    public float moveSpeed = 2f;
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
    public float knockbackMultiplier = 1f;
    public float knockbackToPlayerMultiplier = 5f;  // Multiplicador de knockback causado ao Player
    private bool hasJumped = false;
    public float jumpForce = 5f;  // Força do pulo para alcançar o player em altura

    private int maxHealth;
    private int currentHealth;
    private Transform player;
    private Player playerScript;
    private bool isChasing = false;
    public bool isTutorialEnemy = false;

    private Slider enemyHealthBar;
    private Animator animator;

    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath;

    void Start()
    {
        maxHealth = Random.Range(150, 300);
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
                // Se o jogador estiver muito acima, pular para alcançá-lo
                if (!hasJumped && player.position.y - transform.position.y > 2f)
                {
                    Rigidbody2D rb = GetComponent<Rigidbody2D>();
                    if (rb != null && Mathf.Abs(rb.linearVelocity.y) < 0.1f)
                    {
                        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        hasJumped = true;
                    }
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
        // Aplicar knockback no player ao acertar o ataque
        if (playerScript != null)
        {
            Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                Vector2 knockDir = (player.position - transform.position).normalized;
                playerRb.AddForce(knockDir * knockbackToPlayerMultiplier, ForceMode2D.Impulse);
            }
        }
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            hasJumped = false;
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.AddForce(force * knockbackMultiplier, ForceMode2D.Impulse);
        }
    }

    public float GetKnockbackToPlayerMultiplier()
    {
        return knockbackToPlayerMultiplier;
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
