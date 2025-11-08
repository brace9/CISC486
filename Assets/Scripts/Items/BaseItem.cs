using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;

public class BaseItem : MonoBehaviour
{
    public GameObject holder;
    public Enemy enemy; // if enemy is holding

    public Vector2 enemyUseInterval = new Vector2(0, 3); // how often the enemy uses when targeting, todo make certain items override

    float disabledUntil = -1;

    int uses;

    public virtual void Use()
    {
        uses += 1;
    }

    public bool isDisabled()
    {
        return Time.time <= disabledUntil;
    }

    public void Cooldown(float secs)
    {
        disabledUntil = Time.time + secs;
    }

    public virtual string GetName()
    {
        return "...";
    }
}
