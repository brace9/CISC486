using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownPearl : MonoBehaviour
{
    public float lifetime = 5f;
    public GameObject thrower;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (thrower != null)
            thrower.transform.position = transform.position;

        Destroy(gameObject);
        print("collided with " + collision.gameObject);
    }
}
