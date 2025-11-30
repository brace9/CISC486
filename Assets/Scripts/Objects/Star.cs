using System.Collections;
using UnityEngine;
using Unity.Netcode;
public class Star : NetworkBehaviour
{
    public bool isIdle = true;  // for naturally spawned stars
    public NetworkVariable<bool> collectible = new NetworkVariable<bool>(true);  // server-authoritative

    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return; // Only the server handles collection
        if (!collectible.Value) return;

        if (other.CompareTag("PlayerBody"))
        {
            Player player = other.transform.parent.GetComponent<Player>();
            if (player != null)
            {
                // Update server-authoritative star count
                player.server.GainStar();

                Debug.Log($"Player now has {player.server.stars.Value} stars!");

                // Only naturally spawned stars trigger a respawn
                if (isIdle)
                {
                    GameManager gm = FindObjectOfType<GameManager>();
                    if (gm != null)
                        gm.OnIdleStarCollected();
                }

                // Remove the star from all clients
                NetworkObject netObj = GetComponent<NetworkObject>();
                if (netObj != null)
                    netObj.Despawn(true); // true = destroy on all clients
            }
        }
    }
    // Make star collectible after a delay (for dropped stars)
    public IEnumerator ToggleCollectible(bool enabled, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (IsServer)
            collectible.Value = enabled;
    }
}
