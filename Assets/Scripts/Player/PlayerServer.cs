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

    public void Attack(Vector3 pos, Vector3 dir, float distance)
	{
		if (!IsServer) return;

        if (Physics.Raycast(new Ray(pos, dir), out RaycastHit hit, distance))
        {
            if (hit.collider.TryGetComponent(out Enemy enemy))
            {
                enemy.TakeDamage(player.attackDamage);
            }

            else if (hit.collider.TryGetComponent(out Player pl))
            {
                if (pl != player)
                    pl.server.TakeDamage(player.attackDamage);
            }
        }
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
