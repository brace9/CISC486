using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnderPearl : BaseItem
{
    [Header("Ender Pearl Settings")]
    public float throwStrength = 10f;
    public float upwardForce = 4f;
    public GameObject thrownPearlPrefab;
    public float offset = 1f;


    public override void Use()
    {
        if (isDisabled()) return;

        var rb = holder.GetComponent<Rigidbody>();

        if (enemy != null)
            return; // logistically enemy usage of this item really doesnt make alot of sense, come back to this

        Camera throwerCamera = holder.GetComponentInChildren<Camera>();

        if (throwerCamera == null)
        {
            Debug.LogWarning("camera not found in holder");
            return;
        }

        Transform cameraTransform = throwerCamera.transform;

        Vector3 spawnPosition = cameraTransform.position + cameraTransform.forward * offset;

        GameObject thrownPearl = Instantiate(thrownPearlPrefab, spawnPosition, cameraTransform.rotation);
        thrownPearl.GetComponent<ThrownPearl>().thrower = holder; // set thrower

        Rigidbody pearlRB = thrownPearl.GetComponent<Rigidbody>();

        Vector3 throwDirection = cameraTransform.forward * throwStrength + cameraTransform.up * upwardForce;
        pearlRB.AddForce(throwDirection, ForceMode.VelocityChange);

        base.Use();
        Cooldown(3);
    }
}
