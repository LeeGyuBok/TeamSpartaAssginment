using UnityEngine;

public interface IDamageable
{
    public float MaxHealthPoints { get; }
    public float CurrentHealth { get; }
    public void TakeDamage(float damage);
}
