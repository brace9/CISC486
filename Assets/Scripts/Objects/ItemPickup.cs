using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{

    public BaseItem item;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "PlayerBody")
        {
            Player p = other.transform.parent.GetComponent<Player>();
            p.item = item;
            Destroy(gameObject);
        }
    }
}
