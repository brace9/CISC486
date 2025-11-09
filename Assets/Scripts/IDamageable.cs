using UnityEngine;

public interface IDamageable
{
    /// <summary>
    /// Apply damage to this object. The source parameter is optional context (who caused the damage).
    /// </summary>
    /// <param name="damage">Amount of damage to apply.</param>
    /// <param name="source">GameObject that caused the damage (may be null).</param>
    void TakeDamage(float damage, GameObject source);
}
