using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{

    public KeyCode itemKey = KeyCode.LeftShift;

    public BaseItem item;

    void Update()
    {
        if (item != null && Input.GetKey(itemKey))
        {
            print(item);
            item.Use();
        }
            
    }
}
