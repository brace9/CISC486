using System.Collections;
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
    public override string GetName()
    {
        return "Bash";
    }
}
