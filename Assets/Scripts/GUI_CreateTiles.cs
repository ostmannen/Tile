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

        if (GUILayout.Button("Create Map Default"))
        {
            manager.CreateMap();
        }
        if (GUILayout.Button("Create Map Parallel")){
            manager.CreateMap2();
        }
        if (GUILayout.Button("Destroy All!!")){
            manager.ClearPlacedGameObjects();
        }
    }
}
