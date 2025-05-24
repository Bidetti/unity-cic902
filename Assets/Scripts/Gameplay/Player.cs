using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public GameObject damageTextPrefab;

    public Slider healthBar;
    public TextMeshProUGUI healthText;

    private int[] inventory = new int[5];
    private int inventoryCount = 0;

    public int baseDamage = 10;
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 2f;

    public int maxHealth = 500;
    private int currentHealth;
    private float[] attackCooldowns = { 0.5f, 5f, 7.5f };
    private float[] attackTimers = { 0f, 0f, 0f };

    public bool hasAttacked = false;
    private bool isGrounded = false;
    private bool canDoubleJump = true;
    private float doubleJumpCooldown = 5f;
    private float doubleJumpTimer = 0f;
    private float damageCooldownTimer = 0f;  // Tempo de recarga após dano (0.5s sem poder atacar)

    private Rigidbody2D rb;
    public BoxCollider2D attackCollider;
    public float attackRange = 2f;
    public float attackHeight = 1.0f;
    public float basicKnockback = 5f;
    public float special1Knockback = 7.5f;
    public float special2Knockback = 10f;
    public Animator animator;
    private Tutorial tutorial;

    public delegate void DeathEventHandler();
    public event DeathEventHandler OnDeath;

    public enum ControlMode { Tutorial, Gameplay }
    public ControlMode controlMode = ControlMode.Gameplay;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        tutorial = Object.FindFirstObjectByType<Tutorial>();
        int enemyLayer = LayerMask.GetMask("Enemy");
        attackCollider = gameObject.AddComponent<BoxCollider2D>();
        attackCollider.isTrigger = true;
        UpdateAttackCollider();
        attackCollider.enabled = false;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }

    private void UpdateAttackCollider()
    {
        // Tamanho
        attackCollider.size = new Vector2(attackRange, attackHeight);
        // Offset (metade do range para frente, considerando flip)
        float dir = Mathf.Sign(transform.localScale.x);
        attackCollider.offset = new Vector2(attackRange / 2f * dir, 0);
    }

    IEnumerator PerformAttack(int damage, bool isCritical, float knockback)
    {
        attackCollider.enabled = true;
        hasAttacked = true;
        NotifyEnemies();
        List<IEnemy> hitEnemies = new List<IEnemy>();
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            (Vector2)transform.position + attackCollider.offset,
            attackCollider.size,
            0);

        if (Mathf.Approximately(knockback, basicKnockback))
        {
            // Ataque básico: causa dano apenas no inimigo mais próximo
            float minDistance = Mathf.Infinity;
            IEnemy closestEnemy = null;
            Collider2D closestCollider = null;
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    float dist = Vector2.Distance(hit.transform.position, transform.position);
                    if (dist < minDistance)
                    {
                        IEnemy e = hit.GetComponent<IEnemy>();
                        if (e != null)
                        {
                            minDistance = dist;
                            closestEnemy = e;
                            closestCollider = hit;
                        }
                    }
                }
            }
            if (closestEnemy != null)
            {
                closestEnemy.TakeDamage(damage, isCritical);
                Vector2 knockDir = (closestCollider.transform.position - transform.position).normalized;
                closestEnemy.ApplyKnockback(knockDir * knockback);
            }
        }
        else
        {
            // Ataques especiais: causam dano em todos os inimigos na área de ataque
            foreach (Collider2D hit in hits)
            {
                if (hit.CompareTag("Enemy"))
                {
                    IEnemy enemy = hit.GetComponent<IEnemy>();
                    if (enemy != null && !hitEnemies.Contains(enemy))
                    {
                        hitEnemies.Add(enemy);
                        enemy.TakeDamage(damage, isCritical);
                    }
                    Vector2 knockDir = (hit.transform.position - transform.position).normalized;
                    enemy.ApplyKnockback(knockDir * knockback);
                }
            }
        }

        yield return new WaitForSeconds(0.2f);
        attackCollider.enabled = false;
        hasAttacked = false;
    }

    void Update()
    {
        if (controlMode == ControlMode.Tutorial)
        {
            TutorialControl();
        }
        else
        {
            GameplayControl();
        }

        UpdateAttackTimers();
        UpdateDoubleJumpTimer();
        UpdateDamageCooldown();
    }

    void TutorialControl()
    {
        Move();
        HandleJump();
        HandleAttacks();
    }

    void GameplayControl()
    {
        Move();
        HandleJump();
        HandleAttacks();
    }

    void Move()
    {
        float moveX = Input.GetAxis("Horizontal");

        Vector3 movement = new Vector3(moveX, 0f, 0f);
        transform.position += movement * moveSpeed * Time.deltaTime;

        if (moveX != 0)
        {
            animator.SetBool("isRunning", true);
            transform.localScale = new Vector3(Mathf.Sign(moveX) * 5, 5, 5);
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 0)
            {
                tutorial.OnPlayerAction();
            }
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
            animator.SetBool("isJumping", true);
            if (isGrounded)
            {
                Jump();
                if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 1)
                {
                    tutorial.OnPlayerAction();
                }
            }
            else if (canDoubleJump)
            {
                DoubleJump();
                if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 1)
                {
                    tutorial.OnPlayerAction();
                }
            }
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isGrounded = false;
    }

    void DoubleJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        canDoubleJump = false;
        doubleJumpTimer = doubleJumpCooldown;
    }

    void UpdateDoubleJumpTimer()
    {
        if (!canDoubleJump)
        {
            doubleJumpTimer -= Time.deltaTime;
            if (doubleJumpTimer <= 0)
            {
                canDoubleJump = true;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackCollider != null)
        {
            Gizmos.color = Color.red;
            Vector2 position = (Vector2)transform.position + attackCollider.offset;
            Gizmos.DrawWireCube(position, attackCollider.size);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            if (!hasAttacked)
            {
                Enemy1 enemy = collision.gameObject.GetComponent<Enemy1>();
                if (enemy != null && enemy.attackCollider.enabled)
                {
                    TakeDamage(enemy.baseDamage);
                }
                Enemy2 enemy2 = collision.gameObject.GetComponent<Enemy2>();
                if (enemy2 != null && enemy2.attackCollider.enabled)
                {
                    TakeDamage(enemy2.baseDamage);
                }
            }
        }
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

    void UpdateDamageCooldown()
    {
        if (damageCooldownTimer > 0)
        {
            damageCooldownTimer -= Time.deltaTime;
        }
    }

    void HandleAttacks()
    {
        if (Input.GetKeyDown(KeyCode.Space) && attackTimers[0] <= 0 && damageCooldownTimer <= 0f) // Ataque básico
        {
            Attack();
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 2)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && attackTimers[1] <= 0 && damageCooldownTimer <= 0f) // Ataque especial 1
        {
            SpecialAttack(1, 1.15f); // 15% a mais de dano, Delay de 5s
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 3)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && attackTimers[2] <= 0 && damageCooldownTimer <= 0f) // Ataque especial 2
        {
            SpecialAttack(2, 1.30f); // 30% a mais de dano, Delay de 7.5s
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 4)
            {
                tutorial.OnPlayerAction();
            }
        }
    }

    void Attack()
    {
        animator.SetTrigger("attack");
        int damage = Random.Range(5, 15) + baseDamage;
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }

        float direction = transform.localScale.x > 0 ? 1 : -1;
        attackCollider.offset = new Vector2(attackRange / 2 * direction, 0);

        UpdateAttackCollider();
        StartCoroutine(PerformAttack(damage, isCritical, basicKnockback));
        attackTimers[0] = attackCooldowns[0];
    }

    void SpecialAttack(int attackType, float damageMultiplier)
    {
        string triggerName = "attack" + attackType;
        animator.SetTrigger(triggerName);
        int damage = (int)((Random.Range(5, 15) + baseDamage) * damageMultiplier);
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }

        float direction = transform.localScale.x > 0 ? 1 : -1;
        attackCollider.offset = new Vector2(attackRange / 2 * direction, 0);

        UpdateAttackCollider();

        float kb = attackType == 1 ? special1Knockback : special2Knockback;
        StartCoroutine(PerformAttack(damage, isCritical, kb));
        attackTimers[attackType] = attackCooldowns[attackType];
    }

    public void TakeDamage(int damage, bool isCritical = false)
    {
        animator.SetTrigger("hurt");
        if (damageTextPrefab != null)
        {
            Vector3 textPosition = transform.position + new Vector3(0f, 0.7f, 0f);
            textPosition += new Vector3(Random.Range(-0.2f, 0.2f), 0f, 0f);
            GameObject damageTextObj = Instantiate(damageTextPrefab, textPosition, Quaternion.identity);
            damageText textScript = damageTextObj.GetComponent<damageText>();
            if (textScript != null)
            {
                textScript.SetDamage(damage, isCritical, true);
            }
            else
            {
                damageTextObj.SendMessage("SetDamage", damage);
            }
        }
        currentHealth -= damage;
        healthBar.value = currentHealth;
        healthText.text = currentHealth + "/" + maxHealth;
        damageCooldownTimer = 0.5f;  // Inicia cooldown de 0.5s após sofrer dano

        if (currentHealth <= 0)
        {
            animator.SetTrigger("death");
            currentHealth = 0;
            healthBar.value = currentHealth;
            healthText.text = "0/" + maxHealth;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthText.text = currentHealth + "/" + maxHealth;
        }
    }

    void NotifyEnemies()
    {
        GameObject[] enemyObjects = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject enemyObject in enemyObjects)
        {
            IEnemy enemy = enemyObject.GetComponent<IEnemy>();
            if (enemy != null)
            {
                enemy.StartChasingPlayer();
            }
        }
    }
}
