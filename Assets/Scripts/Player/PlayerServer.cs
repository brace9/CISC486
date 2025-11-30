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

    public void TakeDamage(float damage)
	{
		if (player.invincible || !IsServer) return;

        hp.Value -= damage;

        // At 0 HP, heal to full but drop a star
        if (hp.Value <= 0)
        {
            hp.Value = player.startingHP;
            StartCoroutine(MakeInvincible(player.invincibilitySecs));

            if (stars.Value >= 1)
            {
                GainStar(-1);
                gm.SpawnDroppedStar(transform);
            }
        }
	}

    public void GainStar(int amount = 1)
	{
        stars.Value += amount;

        if (stars.Value < 0) stars.Value = 0;
	}
}
