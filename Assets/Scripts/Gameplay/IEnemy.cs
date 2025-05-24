using UnityEngine;

public interface IEnemy
{
    void StartChasingPlayer();
    void TakeDamage(int damage, bool isCritical = false);
    void ApplyKnockback(Vector2 force);
    float GetKnockbackToPlayerMultiplier();
}
