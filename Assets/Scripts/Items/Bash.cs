using System.Collections;
using UnityEngine;

public class Bash : BaseItem
{
    [Header("Bash Settings")]
    public float forwardStrength = 12f; // horizontal dash speed
    public float upwardStrength = 6f;   // vertical impulse
    public float dashDuration = 0.8f;   // time to keep physics enabled
    public float cooldownSeconds = 3f;

    public override void Use()
    {
        if (isDisabled()) return;

        var rb = holder.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // switch the enemy to physics mode for the dash duration
            if (enemy != null)
                StartCoroutine(enemy.SwitchToRigidbody(dashDuration));

            // set velocity: forward dash + upward impulse
            Vector3 forward = holder.transform.forward;
            Vector3 velocity = forward.normalized * forwardStrength;
            velocity.y = upwardStrength;

            rb.velocity = velocity;

            base.Use();
            Cooldown(cooldownSeconds);
        }
    }
}
