using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    public float startingHP = 3;
    public int stars = 3;
    public float invincibilitySecs = 1;

    float hp;
    bool invincible;

    public Text starText;
    public KeyCode itemKey = KeyCode.LeftShift;
    public KeyCode debugDamageKey = KeyCode.Backspace;
    public BaseItem item;

    GameManager gm;

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
