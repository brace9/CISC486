using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TerrainNavigation))]
public class TerrainNavigationEditor : Editor
{
    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        TerrainNavigation terrainScript = (TerrainNavigation)target;
        if (GUILayout.Button("Build Navmesh")) {
            terrainScript.buildNavMesh();
        }
    }
}