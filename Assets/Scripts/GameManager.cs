using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public GameObject starPrefab;
    public Vector2 starRespawnTime = new Vector2(3, 4); // min-max time for a new star to spawn after one is collected
    public int disableRecentPositions = 3; // don't spawn stars in the last N locations that already had

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
    

}
