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
        // checking for body and not player is probably bad practice but whatever lmao
        if (other.gameObject.name == "Body")
        {
            Debug.Log("Entered zone, state change to target should happen");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Body")
        {
            Debug.Log("Exited zone, state change back to idle should happen");
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