using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 2f;
    public GameObject damageTextPrefab;
    public Transform damageTextPosition;

    private int[] inventory = new int[5];
    private int inventoryCount = 0;

    public int baseDamage = 10;
    public float criticalChance = 0.1f;
    public float criticalMultiplier = 2f;

    public int maxHealth = 500;
    private int currentHealth;
    private float[] attackCooldowns = { 5f, 7.5f, 15f };
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
    }

    void HandleAttacks()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack(1);
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 2)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha1) && attackTimers[0] <= 0)
        {
            SpecialAttack(1, 0);
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 3)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha2) && attackTimers[1] <= 0)
        {
            SpecialAttack(2, 1);
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 4)
            {
                tutorial.OnPlayerAction();
            }
        }
        if (Input.GetKeyDown(KeyCode.Alpha3) && attackTimers[2] <= 0)
        {
            SpecialAttack(3, 2);
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 5)
            {
                tutorial.OnPlayerAction();
            }
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

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right, 1.5f);
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            hasAttacked = true;
            NotifyEnemies();

            Enemy1 enemy = hit.collider.GetComponent<Enemy1>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
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

    void ShowDamage(int damage, bool isCritical)
    {
        GameObject damageText = Instantiate(damageTextPrefab, damageTextPosition.position, Quaternion.identity);
        damageText.GetComponent<Text>().text = damage.ToString();
        if (isCritical)
        {
            damageText.GetComponent<Text>().color = Color.red;
        }
        Destroy(damageText, 1f);
    }

    void PickUpItem(int itemId)
    {
        if (inventoryCount < 5)
        {
            inventory[inventoryCount] = itemId;
            inventoryCount++;
            if (controlMode == ControlMode.Tutorial && tutorial.currentStep == 6)
            {
                tutorial.OnPlayerAction();
            }
        }
        else
        {
            Debug.Log("Inventário cheio!");
        }
    }

    void UseItem(int slot)
    {
        if (slot < inventoryCount)
        {
            // criar logica do item ainda....
            inventory[slot] = 0;
            inventoryCount--;
        }
    }

    public void TakeDamage(int damage)
    {
        ShowDamage(damage, false);
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            // morreu
        }
    }

    public void IncreaseMaxHealth(int amount)
    {
        maxHealth += amount;
        currentHealth = maxHealth;
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