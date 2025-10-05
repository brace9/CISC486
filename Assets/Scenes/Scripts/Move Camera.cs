using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
/*
{
    // Start is called before the first frame update

    public Transform camPostion;
    
    private void update()
    {
        transform.position = camPostion.position;
    }
}
*/
{
    [Header("The object to follow (CameraPos)")]
    public Transform target;

    [Header("Optional offset")]
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        // Move camera to the target position + offset
        transform.position = target.position + offset;

        // Match rotation exactly
        transform.rotation = target.rotation;
    }
}