using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// IDLE: Hang around and wait for a target
public class IdleState : State
{

    public TargetState targetState;
    private Enemy enemy;

    void Awake() => enemy = GetComponent<Enemy>();

    public override State RunCurrentState()
    {
        if (enemy.CanSeePlayer())
            return targetState;

        return this; // stay idle
    }
}