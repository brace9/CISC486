using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetState : State
{
    public bool inAttackRange;
    public AttackState attackState;

    public override State RunCurrentState()
    {
        if(inAttackRange)
        {
            return attackState;
        }
        else
        {
            return this;
        }
    }
}
