/*
This script controls the base functionality for the program. 

Email :: thomas.ir.rasor@gmail.com
*/
using UnityEngine;

public class AppCoreControl : MonoBehaviour
{
	public int frameRateLimit = 90;
	public bool showFps = false;
	public bool enableVsync = false;
	public int maxQueuedFrames = 2;
	public bool pauseOnFocusLoss = false;
	public enum AASetting { None , x2 , x4 , x8 }
	public AASetting antialiasing = AASetting.x4;

	[ContextMenu("Update Settings Now")]
	void Start ()
	{
		Application.runInBackground = !pauseOnFocusLoss;
		Application.targetFrameRate = frameRateLimit;
		if ( enableVsync )
			QualitySettings.vSyncCount = 1;
		else
			QualitySettings.vSyncCount = 0;
		QualitySettings.maxQueuedFrames = maxQueuedFrames;
		QualitySettings.antiAliasing = (int)antialiasing;
    }

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Escape))
			Application.Quit ();
	}

	void OnGUI ()
	{
		if ( !showFps ) return;
		GUILayout.BeginArea( new Rect( 0f , 0f , Screen.width , Screen.height ) );
		GUILayout.Label( "~" + Mathf.RoundToInt( 1f / Time.smoothDeltaTime ) + " fps" );
		GUILayout.EndArea();
	}
}
