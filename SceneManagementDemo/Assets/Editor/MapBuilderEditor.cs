using UnityEngine;
using System.Collections;
using UnityEditor;

using Tribal;

[CustomEditor(typeof(MapBuilderScript))]
public class MapBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
		MapBuilderScript myScript = (MapBuilderScript)target;

		if(GUILayout.Toggle(Map.ForceHexOutlines, "Show Hex Outlines"))
		{
			Map.ShowHexOutlines( true );
		} else
			Map.ShowHexOutlines( false );

        if(GUILayout.Button("Destroy Map"))
        {
            myScript.DestroyMap();
        }

		if(GUILayout.Button("Create Map"))
        {
            myScript.CreateMap();
        }

		if(GUILayout.Button("Settle Map"))
        {
            myScript.SettleMap();
        }

		if(GUILayout.Button("Add Ring"))
        {
            myScript.AddRing();
        }
    }
}

