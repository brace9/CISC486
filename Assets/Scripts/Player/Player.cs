using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Player : NetworkBehaviour
{

    public float startingHP = 3;
    public int stars = 3;
    public float invincibilitySecs = 1;

    float hp;
    bool invincible;

    public Text starText;
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

    GameManager gm;

    public override void OnNetworkSpawn()
	{
        if (gm == null) Start();
        gm.OnSpawnPlayer(this);
	}

    void Start()
    {
        gm = FindObjectOfType<GameManager>();
        hp = startingHP;
        if (starText != null) starText.gameObject.SetActive(true);
    }

    void Update()
    {
        if (starText != null)
        {
            starText.text = $"HP: {hp}\nStars: {stars}\nItem: {(item == null ? "None" : item.GetName())}";
            if (invincible) starText.text += "\n(invincible)";
        }

        if (item != null && Input.GetKey(itemKey))
        {
            print($"Using item: {item}");
            item.Use();
        }

        if (Input.GetKeyDown(debugDamageKey))
        {
            TakeDamage(1);
        }
        
        if(Input.GetKeyDown(attackKey))
        {
            Attack();
        }
    }

    public void Attack()
    {
        if (attacking || !canAttack)
        {
            //print("Unsuccessful Attack");
            return;
        }

        canAttack = false;
        attacking = true;

        Invoke(nameof(ResetAttack), attackSpeed);
        Invoke(nameof(AttackRaycast), 0);

        //print("Successful Attack");
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

        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance))
        {
            if (hit.collider.name == "Enemy")
            {
                Enemy enemy = hit.collider.GetComponent<Enemy>();

                if (enemy != null)
                {
                    enemy.TakeDamage(1);
                }
            }
        }
    }

    public void TakeDamage(float damage)
    {
        if (invincible) return;

        hp -= damage;

        // At 0 HP, heal to full but drop a star
        if (hp <= 0)
        {
            hp = startingHP;
            StartCoroutine(MakeInvincible(invincibilitySecs));

            if (stars >= 1)
            {
                stars -= 1;
                gm.SpawnDroppedStar(transform);
            }
        }
    }

    public IEnumerator MakeInvincible(float time)
    {
        invincible = true;
        yield return new WaitForSeconds(time);
        invincible = false;
    }
}
