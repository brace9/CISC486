using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideOnLoad : MonoBehaviour
{
    void Awake()
	{
        GetComponent<MeshRenderer>().enabled = false;
	}
}
