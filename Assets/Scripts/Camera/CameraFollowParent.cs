using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowParent : MonoBehaviour
{
    [Header("Assign the object to follow")]
    public Transform parentToFollow;

    [Header("Offset from the parent (e.g., head height)")]
    public Vector3 offset = new Vector3(0, 1.6f, 0);

    void LateUpdate()
    {

        // Move cameraPos to follow parent position + offset
        transform.position = parentToFollow.position + offset;

        // Optionally match rotation (so it turns with the parent)
        transform.rotation = parentToFollow.rotation;
    }
}
