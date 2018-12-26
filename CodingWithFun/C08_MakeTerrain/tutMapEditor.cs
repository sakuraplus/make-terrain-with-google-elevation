using UnityEngine;
using UnityEditor;

[CustomEditor( typeof(tutMap))]
public class tutMapEditor:Editor  {

	public override void OnInspectorGUI(){

		DrawDefaultInspector ();
		tutMap tut = (tutMap) target;
		GUILayout.Space (20);

		if (GUILayout.Button ("Try!")) {
			tut.generateTerrain ();
		}

	}

}
