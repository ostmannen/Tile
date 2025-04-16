using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMapManager))]
public class GUI_CreateTiles : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TileMapManager manager = (TileMapManager)target;

        if (GUILayout.Button("Create Map")){
            manager.CreateMap();
        }
    }
}
