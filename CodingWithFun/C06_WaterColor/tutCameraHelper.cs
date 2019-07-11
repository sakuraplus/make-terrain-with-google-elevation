using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


[ExecuteInEditMode]
public class tutCameraHelper : MonoBehaviour {

	[Header("Camera only")]

	[SerializeField]
	Material material;


	void Start () {
		////assign Material as the sharedmaterial of current gameobject while material is null
		if (gameObject.GetComponent<Renderer> () && material==null) {
			material = gameObject.GetComponent<Renderer> ().sharedMaterial;
			//material = gameObject.GetComponent<Renderer> ().material;//Use the instanced temporary material (if exist)
		}
		if (material == null ) {
			Debug.LogWarning ("Material not found: "+gameObject.name);
			return;
		}
	//	GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
		GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;	
	}

	/// <summary>
	/// Raises the render image event(CameraOnly).
	/// </summary>
	void OnRenderImage(RenderTexture source,RenderTexture destination)
	{
		if (material) {
			Graphics.Blit (source, destination, material);
		}
	}

}
