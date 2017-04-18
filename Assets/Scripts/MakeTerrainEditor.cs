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
		if(GUILayout.Button("run"))
		{
			// if button pressed, then call function in script
			_main.EditorCall ();
		}

	


		// //////////////////////////
//		if(GUILayout.Button("print"))
//		{
//			// if button pressed, then call function in script
//			Debug.Log ("oooo");
//			if (EditorUtility.DisplayDialog("????",   "aaaaa?",   "Yes00", "No")) {
//		
//			}
//		}

	}
}
