using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class PlayerServer : NetworkBehaviour
{
    [HideInInspector] public NetworkVariable<float> hp = new NetworkVariable<float>();
    [HideInInspector] public NetworkVariable<int> stars = new NetworkVariable<int>();

    [HideInInspector] public Player player;
    [HideInInspector] public GameManager gm;

    void Awake()
	{
		player = GetComponent<Player>();
	}

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;
        hp.Value = player.startingHP;
    }

    public IEnumerator MakeInvincible(float time)
    {
        player.invincible = true;
        yield return new WaitForSeconds(time);
        player.invincible = false;
    }

    [ServerRpc]
    public void AttackServerRpc(Vector3 pos, Vector3 dir, float distance)
    {
        // SERVER AUTHORITATIVE HIT DETECTION
		if (!IsServer) return;

        if (Physics.Raycast(new Ray(pos, dir), out RaycastHit hit, distance))
        {

            var damageable = hit.collider.GetComponentInParent<IDamageable>();
            if (damageable != null)
            {
                Debug.Log("Hit " + hit.collider.name + ", dealing " + player.attackDamage + " damage.");
                damageable.TakeDamage(player.attackDamage, gameObject);
            }
            else
            {
                Debug.Log("Hit " + hit.collider.name + " but it is not damageable...?");
            }
        }    
        AttackClientRpc(pos, dir, distance);    
    }

    [ClientRpc]
    private void AttackClientRpc(Vector3 pos, Vector3 dir, float distance)
    {
        if (IsOwner) return;

        // TODO: Play attack animation on remote players
        Debug.Log("Remote player attacked.");
    }

    [ServerRpc(RequireOwnership = false)]

    public void TakeDamageServerRpc(float damage)
    {
        TakeDamage(damage);
    }

    public void TakeDamage(float damage)
	{
		if (player.invincible || !IsServer) return;

        hp.Value -= damage;
        Debug.Log($"Player took {damage} damage, HP now {hp.Value}");

        // At 0 HP, heal to full but drop a star
        if (hp.Value <= 0)
        {
            hp.Value = player.startingHP;
            Debug.Log("Player died, dropping a star.");
            StartCoroutine(MakeInvincible(player.invincibilitySecs));

            if (stars.Value >= 1)
            {
                GainStar(-1);

                // Use singleton reference for the server
                if (GameManager.Instance != null)
                    GameManager.Instance.SpawnDroppedStar(transform);
                else
                    Debug.LogWarning("Server GameManager instance is null!");
            }
        }
	}

    public void GainStar(int amount = 1)
	{
        stars.Value += amount;

        if (stars.Value < 0) stars.Value = 0;
	}
}
