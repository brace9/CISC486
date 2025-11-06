using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    
    public float xSpeed = 0f;
    public float ySpeed = 200f;
    public float zSpeed = 0f;

    void Update() {
        var dt = Time.deltaTime;
        transform.Rotate(xSpeed * dt, ySpeed * dt, zSpeed * dt);
    }
}
