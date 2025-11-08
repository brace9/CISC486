using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Natural Stars")]
    public GameObject starPrefab;
    public Vector2 starRespawnTime = new Vector2(3, 4); // min-max time for a new star to spawn after one is collected
    public int disableRecentPositions = 3; // don't spawn stars in the last N locations that already had

    [Header("Dropped Stars")]
    public GameObject droppedStarPrefab;
    public float droppedUpwardsForce = 10.0f;
    public float droppedSidewaysForce = 5.0f;
    public float droppedEnableTime = 0.6f; // Time before you can collect the star

    List<Vector3> starPositions = new();
    List<Vector3> recentStarPositions = new();

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

    void Start()
    {
        SpawnStar();
    }

    public void OnIdleStarCollected()
    {
        Invoke(nameof(SpawnStar), Random.Range(starRespawnTime.x, starRespawnTime.y));
    }

    void SpawnStar()
    {
        var spawnLocations = starPositions.Except(recentStarPositions).ToArray();
        Vector3 pos = spawnLocations[Random.Range(0, spawnLocations.Length)];

        recentStarPositions.Add(pos);
        if (recentStarPositions.Count > disableRecentPositions)
            recentStarPositions.RemoveAt(0);

        Instantiate(starPrefab, pos, Quaternion.identity);
        print($"New star spawned at {pos}");
    }

    public void SpawnDroppedStar(Transform loc)
    {
        if (loc == null || droppedStarPrefab == null) return;

        var dropped = Instantiate(droppedStarPrefab, loc.position, Quaternion.identity);

        Vector3 force = (Vector3.up * droppedUpwardsForce)
            + new Vector3(Random.Range(-1f, 1f), 0f, Random.Range(-1f, 1f)).normalized * droppedSidewaysForce;

        dropped.GetComponent<Rigidbody>().AddForce(force, ForceMode.Impulse);

        StartCoroutine(dropped.transform.GetChild(0).GetComponent<Star>().ToggleCollectible(true, droppedEnableTime));
    }

}
