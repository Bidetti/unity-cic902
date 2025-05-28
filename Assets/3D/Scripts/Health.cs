using UnityEngine;
using System;

public class Health : MonoBehaviour
{
    public int maxHealth = 100;
    [HideInInspector] public int currentHealth;
    public event Action OnHurt;
    public event Action OnDeath;
    private bool dead;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        if (dead) return;
        currentHealth -= dmg;
        OnHurt?.Invoke();
        if (currentHealth <= 0)
        {
            dead = true;
            currentHealth = 0;
            OnDeath?.Invoke();
        }
    }
}
