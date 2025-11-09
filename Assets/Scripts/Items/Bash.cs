using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bash : BaseItem
{
    [Header("Bash Settings")]

    /*
    public float forwardStrength = 12f; // horizontal dash speed
    public float upwardStrength = 2f;   // vertical impulse
    public float dashDuration = 1f;   // time to keep physics enabled
    public float cooldownSeconds = 3f;
    public float dashDistance = 200f;       // how far the dash goes
    public float downwardForce = 2f;     // extra downward force for snappy horizontal dash
    */
    public float dashSpeed = 400f;         // horizontal dash speed
    public float dashDuration = 0.00001f;       // duration of the dash
    public float upwardStrength = 0.3f;   // optional small hop
    public float cooldownSeconds = 3f;


    public float bashDamage = 1f;           // damage per hit
    public float damageRadius = 1f;      // how far to check for enemies around player
    public float damageInterval = 0.05f;   // how often to check while dashing
    public LayerMask damageLayer = ~0;     // set in inspector to only enemy layer(s)
    public bool onlyDamageOncePerEnemy = true; // avoid multi-hitting same enemy repeatedly in a dash

    public override void Use()
    {
        if (isDisabled()) return;

        var rb = holder.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 forward;
            // switch the enemy to physics mode for the dash duration
            if (enemy != null)
            {
                StartCoroutine(enemy.SwitchToRigidbody(dashDuration));
                forward = holder.transform.forward;
            }
            else
            {
                print("Bash used by player");
                // player bash
                Camera throwerCamera = holder.GetComponentInChildren<Camera>();
                Transform fpc = throwerCamera.transform;

                forward = fpc.forward;
                forward.y = 0f;             // keep player dash horizontal
                forward.Normalize();
            }

            // optional small hop
            rb.AddForce(Vector3.up * upwardStrength, ForceMode.VelocityChange);

            // start damage scanning for the dash duration
            StartCoroutine(DashDamageLoop(holder, dashDuration));
            StartCoroutine(HorizontalDash(rb, forward, dashSpeed, dashDuration));
            base.Use();
            Cooldown(cooldownSeconds);
        }
    }


    private IEnumerator HorizontalDash(Rigidbody rb, Vector3 direction, float speed, float duration)
    {
        float timer = 0f;
        float originalDrag = rb.drag;
        rb.drag = 0f;

        while (timer < duration)
        {
            timer += Time.fixedDeltaTime;

            // Move Rigidbody forward by speed * deltaTime (horizontal only)
            Vector3 move = direction * speed * Time.fixedDeltaTime;
            move.y = 0f; // keep horizontal
            rb.MovePosition(rb.position + move);

            yield return new WaitForFixedUpdate();
        }



        rb.drag = originalDrag;
    }

private IEnumerator DashDamageLoop(GameObject owner, float duration)
{
    float timer = 0f;
    // track already damaged enemies in this dash
    var hitSet = onlyDamageOncePerEnemy ? new HashSet<GameObject>() : null;

    while (timer < duration)
    {
        // Use owner position as center; you can offset or use a dedicated hitpoint transform
        Vector3 center = owner.transform.position;

        // Find colliders within radius; use damageLayer to filter to enemy layer
        Collider[] cols = Physics.OverlapSphere(center, damageRadius, damageLayer, QueryTriggerInteraction.Ignore);

        foreach (var c in cols)
        {
            GameObject go = c.gameObject;
            // get the IDamageable or Enemy component
            var dmg = go.GetComponentInParent<IDamageable>();
            if (dmg != null)
            {
                if (hitSet != null)
                {
                    if (hitSet.Contains(go)) continue;
                    hitSet.Add(go);
                }

                // apply damage, pass owner so the target knows source
                dmg.TakeDamage(bashDamage, owner);
            }
            else
                {
                // fallback: try Enemy component directly, can be cut if IDamageable works well
                // fallbacks here only work for enemies, not anything else we might want to damage
                var enemy = go.GetComponentInParent<Enemy>();
                if (enemy != null)
                {
                    if (hitSet != null)
                    {
                        if (hitSet.Contains(enemy.gameObject)) continue;
                        hitSet.Add(enemy.gameObject);
                    }

                    // fallback to the Enemy's single-arg API (older code path)
                    enemy.TakeDamage(bashDamage);
                }
            }
        }

        timer += damageInterval;
        yield return new WaitForSeconds(damageInterval);
    }
}


    public override string GetName()
    {
        return "Bash";
    }
}
