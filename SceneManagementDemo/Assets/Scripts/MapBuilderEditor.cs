using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(MapBuilderScript))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
		MapBuilderScript myScript = (MapBuilderScript)target;
        if(GUILayout.Button("Destroy Map"))
        {
            myScript.DestroyMap();
        }

		if(GUILayout.Button("Create Map"))
        {
            myScript.CreateMap();
        }

		if(GUILayout.Button("Add Ring"))
        {
            myScript.AddRing();
        }
    }
}

