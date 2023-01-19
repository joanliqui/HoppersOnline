using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapBuilder))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapBuilder builder = (MapBuilder)target; 
        base.OnInspectorGUI();

        GUILayout.BeginHorizontal();
        if (GUILayout.Button("AddZone"))
        {
            builder.AddZone();
        }

        GUILayout.EndHorizontal();
    }
}
