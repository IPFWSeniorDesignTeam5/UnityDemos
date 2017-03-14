using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(HexBuilderScript))]
public class HexMapEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
		HexBuilderScript myScript = (HexBuilderScript)target;
        if(GUILayout.Button("Settle"))
        {
            myScript.Settle();
        }
    }
}