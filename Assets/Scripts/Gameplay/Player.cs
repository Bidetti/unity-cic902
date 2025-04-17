using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public GameObject damageTextPrefab;
    public Transform damageTextPosition;

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

    private Rigidbody2D rb;
    private Tutorial tutorial;

    public enum ControlMode { Tutorial, Gameplay }
    public ControlMode controlMode = ControlMode.Gameplay;

    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        tutorial = Object.FindFirstObjectByType<Tutorial>();
        int enemyLayer = LayerMask.GetMask("Enemy");
        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f, enemyLayer);

        if (healthBar != null)
        {
            healthBar.maxValue = maxHealth;
            healthBar.value = currentHealth;
            healthText.text = currentHealth + "/" + maxHealth;
        }
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

        if (moveX != 0 && controlMode == ControlMode.Tutorial && tutorial.currentStep == 0)
        {
            tutorial.OnPlayerAction();
        }
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.W))
        {
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(10);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Player collided with wall.");
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

    void HandleAttacks()
    {
        if (Input.GetKeyDown(KeyCode.Space) && attackTimers[0] <= 0) // Ataque básico
        {
            Attack(1, 0.5f); // Delay de 0.5s
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 2)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && attackTimers[0] <= 0) // Ataque especial 1
        {
            SpecialAttack(1, 0, 1.15f); // 15% a mais de dano, Delay de 5s
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 3)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && attackTimers[1] <= 0) // Ataque especial 2
        {
            SpecialAttack(2, 1, 1.30f); // 30% a mais de dano, Delay de 7.5s
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 4)
            {
                tutorial.OnPlayerAction();
            }
        }
    }

    void Attack(int attackType, float delay)
    {
        int damage = Random.Range(5, 15) + baseDamage;
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }

        Debug.Log($"Player Attack: Type={attackType}, Damage={damage}, IsCritical={isCritical}");
        Debug.DrawRay(transform.position, transform.right * 1.5f, Color.red, 1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            hasAttacked = true;
            NotifyEnemies();

            Enemy1 enemy = hit.collider.GetComponent<Enemy1>();
            if (enemy != null)
            {
                Debug.Log($"Player hit Enemy: {enemy.name}, Damage={damage}");
                enemy.TakeDamage(damage);
                ShowDamage(damage, isCritical);
            }
            else
            {
                Debug.LogWarning("Player hit an object with tag 'Enemy' but no Enemy1 script was found.");
            }
        }
        else
        {
            Debug.Log("Player Attack missed or no enemy in range.");
        }
        attackTimers[0] = delay;
    }

    void SpecialAttack(int attackType, int index, float damageMultiplier)
    {
        int damage = (int)((Random.Range(5, 15) + baseDamage) * damageMultiplier);
        bool isCritical = Random.value < criticalChance;
        if (isCritical)
        {
            damage = (int)(damage * criticalMultiplier);
        }

        Debug.Log($"Player Special Attack: Type={attackType}, Damage={damage}, IsCritical={isCritical}");
        Debug.DrawRay(transform.position, transform.right * 1.5f, Color.blue, 1f);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            hasAttacked = true;
            NotifyEnemies();

            Enemy1 enemy = hit.collider.GetComponent<Enemy1>();
            if (enemy != null)
            {
                Debug.Log($"Player hit Enemy: {enemy.name}, Damage={damage}");
                enemy.TakeDamage(damage);
                ShowDamage(damage, isCritical);
            }
            else
            {
                Debug.LogWarning("Player hit an object with tag 'Enemy' but no Enemy1 script was found.");
            }
        }
        else
        {
            Debug.Log("Player Special Attack missed or no enemy in range.");
        }
        attackTimers[index] = attackCooldowns[index];
    }

    void ShowDamage(int damage, bool isCritical)
    {
        if (damageTextPrefab != null && damageTextPosition != null)
        {
            GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
            TextMeshProUGUI textComponent = damageText.GetComponent<TextMeshProUGUI>();
            if (textComponent != null)
            {
                textComponent.text = damage.ToString();
                textComponent.color = isCritical ? Color.red : Color.white;
            }
            Destroy(damageText, 1f);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"Player TakeDamage: Damage={damage}, CurrentHealth={currentHealth}");
        ShowDamage(damage, false);
        currentHealth -= damage;
        healthBar.value = currentHealth;
        if (currentHealth <= 0)
        {
            Debug.Log("Player died.");
            // morreu
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
            Enemy1 enemy = enemyObject.GetComponent<Enemy1>();
            if (enemy != null)
            {
                enemy.StartChasingPlayer();
            }
        }
    }
}