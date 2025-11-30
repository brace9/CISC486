using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    [Header("Player")]
    public Transform p1Start;
    public Transform p2Start;
    public Text starText;

    [Header("Natural Stars")]
    public NetworkObject starPrefab;
    public Vector2 starRespawnTime = new Vector2(3, 4); // min-max time for a new star to spawn after one is collected
    public int disableRecentPositions = 3; // don't spawn stars in the last N locations that already had

    [Header("Dropped Stars")]
    public NetworkObject droppedStarPrefab;
    public float droppedUpwardsForce = 10.0f;
    public float droppedSidewaysForce = 5.0f;
    public float droppedEnableTime = 0.6f; // Time before you can collect the star

    List<Vector3> starPositions = new();
    List<Vector3> recentStarPositions = new();

    public static GameManager Instance;


    void Awake()
    {
        // Store positions of stars on the map so we know where to spawn them
        var stars = GameObject.FindGameObjectsWithTag("Star");

        foreach (var star in stars)
        {
            starPositions.Add(star.transform.position);
            Destroy(star);
        }
    }

    public override void OnNetworkSpawn()
    {

        Debug.Log("GameManager OnNetworkSpawn called.");
        // Only run this on the server
        if (!IsServer) SpawnStar();
        if (IsServer) Instance = this; // server-only singleton

        Debug.Log("Server initializing GameManager...");

        // Start spawning stars
        SpawnStar();
    }

    public void StartGame()
    {
        if (!IsServer) return;

        var zones = GameObject.FindGameObjectsWithTag("EnemyZone");
        foreach (var zone in zones)
            zone.GetComponent<EnemyZone>().CreateEnemy();
    }

    // Called by the star itself when collected
    public void OnIdleStarCollected()
    {
        if (!IsServer) return;
        Invoke(nameof(SpawnStar), Random.Range(starRespawnTime.x, starRespawnTime.y));
    }

    void SpawnStar()
    {
        if (!IsServer) return;
        Debug.Log("Spawning new star...");

        var spawnLocations = starPositions.Except(recentStarPositions).ToArray();
        if (spawnLocations.Length < 1) {
                if (spawnLocations.Length < 1)
    {
        Debug.Log("No spawn locations available!");
        return;
    }}

        Vector3 pos = spawnLocations[Random.Range(0, spawnLocations.Length)];

        recentStarPositions.Add(pos);
        if (recentStarPositions.Count > disableRecentPositions)
            recentStarPositions.RemoveAt(0);

        NetworkObject newStar = Instantiate(starPrefab, pos, Quaternion.identity);
        newStar.Spawn(); // NETWORK SPAWN
        print($"New star spawned at {pos}");
    }

    /*
    public void SpawnDroppedStar(Vector3 position)
    {
        if (!IsServer) return;
        if (droppedStarPrefab == null) return;

        NetworkObject dropped = Instantiate(droppedStarPrefab, position, Quaternion.identity);
        dropped.Spawn();

        Vector3 force = (Vector3.up * droppedUpwardsForce)
            + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * droppedSidewaysForce;

        dropped.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        // Make collectible after delay
        StartCoroutine(dropped.transform.GetChild(0).GetComponent<Star>().ToggleCollectible(true, droppedEnableTime));
    }*/
    public void SpawnDroppedStar(Transform loc)
{
    if (!IsServer) return;
    if (droppedStarPrefab == null || loc == null) return;

    // Spawn the NetworkObject
    NetworkObject dropped = Instantiate(droppedStarPrefab, loc.position, Quaternion.identity);
    dropped.Spawn();

    // Apply upward + random sideways force
    Vector3 force = (Vector3.up * droppedUpwardsForce)
        + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * droppedSidewaysForce;

    Rigidbody rb = dropped.GetComponent<Rigidbody>();
    if (rb != null)
        rb.AddForce(force, ForceMode.Impulse);

    // Make collectible after delay
    Star starScript = dropped.GetComponent<Star>();
    if (starScript != null)
        StartCoroutine(starScript.ToggleCollectible(true, droppedEnableTime));
}

}
