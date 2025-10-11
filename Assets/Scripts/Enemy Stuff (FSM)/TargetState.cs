using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TARGET: Enemy pursues target in range
public class TargetState : State
{
    public AttackState attackState;
    public IdleState idleState;
    private Enemy enemy;

    void Awake() => enemy = GetComponent<Enemy>();

    // distance at which we 'engage' and transition into the AttackState (can be larger than immediate attack range)
    public float engageDistance = 6f;

    public override State RunCurrentState()
    {
        // If within the engage distance, transition into AttackState (AttackState will handle moving to exact pre-attack pos)
        if (enemy != null && enemy.player != null)
        {
            float dist = Vector3.Distance(enemy.transform.position, enemy.player.transform.position);
            if (dist <= engageDistance)
                return attackState;
        }

        if (!enemy.CanSeePlayer())
            return idleState;

        return this; // keep chasing
    }
}
