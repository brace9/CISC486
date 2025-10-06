using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State : MonoBehaviour
{
    public abstract State RunCurrentState();
}

// IDLE: Hang around and wait for a target
public class IdleState : State
{

    public bool canSeePlayer;
    public TargetState targetState;

    public override State RunCurrentState()
    {
        if (canSeePlayer)
        {
            return targetState;
        }
        else
        {
            return this;
        }
    }
}

// TARGET: Enemy pursues target in range
public class TargetState : State
{
    public bool inAttackRange;
    public AttackState attackState;

    public override State RunCurrentState()
    {
        if (inAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }
}


// ATTACK: Fight target
public class AttackState : State
{
    public override State RunCurrentState()
    {
        Debug.Log("I have attacked");
        return this;
    }
}