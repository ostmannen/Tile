using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ObjectPool))]

public class GUI_CreateObjectPool : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ObjectPool manager = (ObjectPool)target;

        if (GUILayout.Button("Create Object Pool"))
        {
            manager.CreatePoolItems();
        }
        if (GUILayout.Button("Return Object Pool"))
        {
            manager.ReturnAll();
        }
        if (GUILayout.Button("Destroy All Objects!"))
        {
            manager.DestroyAll();
        }
        
    }
}
