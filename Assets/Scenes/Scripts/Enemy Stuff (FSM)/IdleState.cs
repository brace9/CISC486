using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
