using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        // checking for body and not player is probably bad practice but whatever lmao
        if (other.gameObject.name == "Body")
        {
            Debug.Log("Entered zone, state change to target should happen");
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == "Body")
        {
            Debug.Log("Exited zone, state change back to idle should happen");  
        }
    }
}
