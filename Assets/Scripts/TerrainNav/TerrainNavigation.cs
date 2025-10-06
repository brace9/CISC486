using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.AI.Navigation;

public class TerrainNavigation : MonoBehaviour
{
    public bool autoDetectObjects = true;
    public bool autoBakeNavmesh = false;
    NavMeshSurface surface;

    void Start() {
        if (autoBakeNavmesh) buildNavMesh();
    }

    public void buildNavMesh() {
        if (autoDetectObjects) {
            Collider[] colliders = Physics.OverlapSphere(transform.position, 5000);
            foreach (var col in colliders) {
                if (col.GetType() != typeof(TerrainCollider) && col.gameObject.tag != "Player" && !col.gameObject.GetComponent<NavMeshAgent>() && !col.gameObject.GetComponent<NavMeshObstacle>()) {
                    col.gameObject.AddComponent(typeof(NavMeshObstacle));
                }
            }
        }

        surface = GetComponent<NavMeshSurface>();
        if (!surface) surface = gameObject.AddComponent(typeof(NavMeshSurface)) as NavMeshSurface;
        surface.BuildNavMesh();
    }
}
