using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public bool isIdle = true;  // for naturally spawned stars
    public bool collectible = true;

    void OnTriggerStay(Collider other)
    {
        if (collectible && other.gameObject.tag == "PlayerBody")
        {
            Player p = other.transform.parent.GetComponent<Player>();
            p.stars += 1;

            print($"Player now has {p.stars} stars!");

            // Tell GameManager to spawn a new star in a few seconds (naturally spawned stars only)
            if (isIdle)
                FindObjectOfType<GameManager>().OnIdleStarCollected();

            Destroy(gameObject);
        }
    }

    public IEnumerator ToggleCollectible(bool enabled, float time)
    {
        yield return new WaitForSeconds(time);
        collectible = enabled;
    }
}
