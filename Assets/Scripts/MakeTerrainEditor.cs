using UnityEngine;
using System.Collections;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(main ))]
public class MakeTerrainEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		main _main = (main)target;



		// add a custom button to the Inspector component
		if(GUILayout.Button("combine meshs"))
		{
			_main.meshcombine ();//EditorCall ();//editor 不调用协程
		}
		if(GUILayout.Button("save a prefab"))
		{
			_main.saveprefab  ();
		}

	}
}
