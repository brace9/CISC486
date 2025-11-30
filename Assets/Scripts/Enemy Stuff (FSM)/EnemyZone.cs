using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    [HideInInspector] public Enemy enemy;

    Collider trigger;

    void Awake()
    {
        trigger = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!enemy.targetPlayer && other.gameObject.tag == "PlayerBody" && !enemy.IsFleeing())
        {
            enemy.targetPlayer = other.gameObject; // Player body
            enemy.ChangeState(EnemyState.TARGET);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemy.targetPlayer && !enemy.IsFleeing())
        {
            enemy.targetPlayer = null;
            enemy.ChangeState(EnemyState.IDLE);
        }
    }

    public Vector3 GetRandomPoint()
    {
        // Pick a random point in the range
        var point = new Vector3(
            Random.Range(trigger.bounds.min.x, trigger.bounds.max.x),
            trigger.bounds.max.y,
            Random.Range(trigger.bounds.min.z, trigger.bounds.max.z)
        );

        // Raycast down to get Y position
        Physics.Raycast(point, Vector3.down, out RaycastHit hit, trigger.bounds.max.y, PlayerMovement.WhatIsGround);
        point.y = hit.point.y;

        return point;
    }
}