using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindCharge : BaseItem
{
    float strength = 15.0f;

    public override void Use()
    {
        if (isDisabled()) return;

        var rb = holder.GetComponent<Rigidbody>();

        if (rb != null)
        {
            if (enemy != null)
                StartCoroutine(enemy.SwitchToRigidbody(1));

            var velocity = rb.velocity;
            velocity.y = strength;

            rb.velocity = velocity;

            base.Use();
            Cooldown(3);
        }
    }

    public override string GetName()
    {
        return "Wind Charge";
    }
}
