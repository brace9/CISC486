using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class Player : NetworkBehaviour, IDamageable
{

    public float startingHP = 3;
    public float invincibilitySecs = 1;

    public bool invincible;

    public MeshRenderer body;
    public KeyCode itemKey = KeyCode.LeftShift;
    public KeyCode debugDamageKey = KeyCode.Backspace;
    public KeyCode attackKey = KeyCode.Mouse0;
    public BaseItem item;

    [Header("Attacking")]
    public float attackDistance = 3f;
    public float attackSpeed = 1;
    public int attackDamage = 1;

    public bool attacking = false;
    public bool canAttack = true;

    [Header("Networking")]
    public PlayerServer server;
    public Material hostSkin;
    public Material clientSkin;
    public GameObject[] disableForOthers;

    GameManager gm;

    public override void OnNetworkSpawn()
	{
        body.material = IsHost ? hostSkin : clientSkin;
        server = GetComponent<PlayerServer>();

        // Networking: Only control your own player
        if (!IsOwner)
		{
            {
                foreach (var obj in disableForOthers)
                    obj.SetActive(false);
            }
            return;
		} 

        gm = FindObjectOfType<GameManager>();
        server.gm = gm;

        transform.position = !IsClient ? gm.p1Start.position : gm.p2Start.position;

        if (gm.starText != null) gm.starText.gameObject.SetActive(true);

        if (IsHost)
            gm.StartGame();
	}

    void Update()
    {
        if (!IsOwner) return;  // Networking: Only control your own player

        if (gm.starText != null)
        {
            gm.starText.text = $"HP: {server.hp.Value}\nStars: {server.stars.Value}"; // \nItem: {(item == null ? "None" : item.GetName())}
            if (invincible) gm.starText.text += "\n(invincible)";
        }

        if (item != null && Input.GetKey(itemKey))
        {
            print($"Using item: {item}");
            item.Use();
        }

        if (Input.GetKeyDown(debugDamageKey))
        {
            server.TakeDamage(1);
        }
        
        if(Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (attacking || !canAttack) return;

        canAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), 0);
        
    }

    void ResetAttack()
    {
        attacking = false;
        canAttack = true;
    }

    void AttackRaycast()
    {
        Camera playerCam = GetComponentInChildren<Camera>();
        Transform cameraTransform = playerCam.transform;

        server.AttackServerRpc(cameraTransform.position, cameraTransform.forward, attackDistance);
    
    
    }

    public void TakeDamage(float damage)
    {
        server.TakeDamageServerRpc(damage);
    }

    public void TakeDamage(float damage, GameObject source)
    {
        server.TakeDamageServerRpc(damage);
    }

}
