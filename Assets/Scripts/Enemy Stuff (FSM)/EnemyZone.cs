using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyZone : NetworkBehaviour
{
    public GameObject enemyPrefab;
    public Transform spawnPosition;
    [HideInInspector] public Enemy enemy;

    Collider trigger;

    public override void OnNetworkSpawn()
    {
        if (IsServer) CreateEnemy();
        print($"IsServer = {IsServer}");
    }

    void Awake()
    {
        trigger = GetComponent<Collider>();
    }

    public void CreateEnemy()
	{
        if (!IsServer) return;

		GameObject enemyObj = Instantiate(enemyPrefab, spawnPosition.position, Quaternion.identity);
        NetworkObject netObj = enemyObj.GetComponent<NetworkObject>();
        netObj.Spawn();

        enemy = enemyObj.GetComponent<Enemy>();
        enemy.SetZoneClientRpc(new NetworkObjectReference(GetComponent<NetworkObject>()));
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsServer || enemy == null) return;

        if (!enemy.targetPlayer && other.gameObject.tag == "PlayerBody" && !enemy.IsFleeing())
        {
            enemy.targetPlayer = other.gameObject; // Player body
            enemy.ChangeState(EnemyState.TARGET);
        }
        
    }

    void OnTriggerExit(Collider other)
    {  
        if (!IsServer || enemy == null) return;
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