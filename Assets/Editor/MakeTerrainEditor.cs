using UnityEngine;
using System.Collections;
using System;
using System.IO ;
using UnityEditor; // this is needed since this script references the Unity Editor

[CustomEditor(typeof(main ))]
public class MakeTerrainEditor : Editor { // extend the Editor class

	// called when Unity Editor Inspector is updated
	public override void OnInspectorGUI()
	{
		if(GUILayout.Button("get elevation API key"))
		{
			Application.OpenURL ("https://developers.google.com/maps/documentation/elevation/start?");
		}
		if(GUILayout.Button("get staticmap API key"))
		{
			Application.OpenURL ("https://developers.google.com/maps/documentation/static-maps/");
		}

		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		main _main = (main)target;

		GUILayout.Label ("mesh size="+_main.size.ToString () );


		if(GUILayout.Button("combine meshs"))
		{
			meshcombine (_main._newmeshobj,_main.arrTrr );
		}
		if(GUILayout.Button("save as prefab"))
		{
			savemesh   (_main._newmeshobj);
			saveprefab (_main._newmeshobj );
		}
		if(GUILayout.Button("save as mesh only"))
		{
			savemesh   (_main._newmeshobj);
		}
	
		//测试用
		GUILayout.Label ("test" );
		if(GUILayout.Button("test"))
		{
			Debug.Log ("num=" + main.NumComplete);
			Debug.Log( DateTime.Now.ToString("HH-mm"));
			meshcombinetest (_main._newmeshobj,_main.arrTrr );
//			if (EditorUtility.DisplayDialog ("Delete all player preferences?",
//				    "Are you sure you want to delete all the player preferences, this action cannot be undone?",
//				    "Yes",
//				    "No")) {
//				Debug.Log ("uuuuuu");
//			}	

		}
	
	}


	///测试用
	void meshcombinetest(GameObject _newmeshobj,GameObject[] arrTrr)
	{
		int i = 1;

		Debug.Log ("num=" + main.NumComplete);
		if (arrTrr [i].GetComponent<MeshFilter > ()) {
			Debug.Log("test mesh="+arrTrr [i].GetComponent<MeshFilter >().mesh );
		}
		if (arrTrr [i].GetComponent<MeshRenderer > ()) {
			Debug.Log ("test shm="+arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial);
		}

		if (arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial.GetTexture ("_MainTex")) {
			Debug.Log("test tx="+arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial.GetTexture ("_MainTex") );
		}

		if (main.NumComplete < arrTrr.Length ) {
			Debug.LogWarning ("wait");
			return;
		}
		Debug.Log (GameObject.Find ("TRRMAG"));

	} 
	/// 
	/// 

	void meshcombine(GameObject _newmeshobj,GameObject[] arrTrr)
	{
		Debug.Log ("num c+e=" + main.NumComplete+"+"+main.NumError );
		//没加载完不许合成
		if ((main.NumComplete < arrTrr.Length) && (main.NumComplete + main.NumError < 9)) {
			Debug.LogWarning ("wait");
			if (EditorUtility.DisplayDialog ("wait",
				    "loading is not complete", "Yes")) {
				//Debug.Log ("uuuuuu");
				return;
			}
		} else if (main.NumComplete + main.NumError >= 9) {
			//全部加载完或错误
			if (EditorUtility.DisplayDialog ("!",
				"There is something wrong in loading data,do you want to continue?", "Yes","No")) {
				Debug.Log ("go on");

			}else{
				return;
			}
		
		}


		Debug.Log (GameObject.Find ("TRRMAG"));
		if (GameObject.Find ("TRRMAG")) {
			Debug.Log ("exist");
			//MeshRenderer[] _meshrenders=new MeshRenderer[arrTrr.Length ] ;
			Material[] _materials = new Material[arrTrr.Length ];
			Texture2D[] _textures = new Texture2D[arrTrr.Length];
			for (int i = 0; i < arrTrr.Length; i++) {
				if (arrTrr [i].GetComponent<MeshRenderer > () && arrTrr [i].GetComponent<MeshFilter > ()) {
					if (arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial.GetTexture ("_MainTex")) {
						//当读图失败或不读图时，不管这块的材质
						_materials [i] = arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial;

						Texture2D tx = _materials [i].GetTexture ("_MainTex") as Texture2D; 
						Texture2D tx2D = new Texture2D (tx.width, tx.height, TextureFormat.ARGB32, false);  
						var tmp = tx.GetPixels (0, 0, tx.width, tx.height);
						tx2D.SetPixels (tmp);  
						tx2D.Apply ();  
						_textures [i] = tx2D;  
					}
				
				} else {
					_textures [i] = new Texture2D (10, 10, TextureFormat.ARGB32, false);//
				}
			}
			Debug.Log ("mat="+_materials [0]);

		
			//---------------- 合并 Mesh -------------------------  

			//获取arrtrr中所有MeshFilter组件  

			//新建一个gameobj  
			//_newmeshobj=new GameObject();
			_newmeshobj.name = "CombinedMesh";
			_newmeshobj.AddComponent<MeshRenderer > ();
			_newmeshobj.AddComponent<MeshFilter> ();

			_newmeshobj.GetComponent<MeshFilter>().mesh = new Mesh();   

			//为合并后的新Mesh指定材质 ------------------------------  
			Texture2D _texture = new Texture2D(2048,2048);  
			Rect[] _rects = _texture.PackTextures(_textures, 0, 2048);  
			byte[] combineTex = _texture.EncodeToPNG ();
			string filepathMaterial = "Assets/Resources/" + main.savefiledate;
			if (! Directory.Exists(filepathMaterial)) 
			{
				Directory.CreateDirectory(filepathMaterial);
			}
			Debug.Log ("filepathMaterial=" + filepathMaterial);
			string savematDate = DateTime.Now.ToString ("yyyyMMddHHmm");
			string filenameMat="combineTex"+savematDate;
			File.WriteAllBytes (filepathMaterial+"/"+filenameMat+".png",combineTex);
			//File.WriteAllBytes ("Assets/Resources/combineTex.png",combineTex);
			AssetDatabase.Refresh ();
			Debug.Log ("filenameMat=" + filenameMat);
			Debug.Log ("load"+Resources.Load ("000"));
			_texture = Resources.Load (main.savefiledate+"/"+filenameMat) as Texture2D;
			//_texture = Resources.Load ("combineTex") as Texture2D;
			Debug.Log("_texture="+_texture );


			Material materialNew = new Material(Shader.Find("Standard"));//(_materials[0].shader);  
			//materialNew.CopyPropertiesFromMaterial(_materials[0]);  
			//这样好像选择不读图的时候就不会出错了

			materialNew.SetTexture("_MainTex", _texture); 
			AssetDatabase.CreateAsset (materialNew,filepathMaterial +"/combineMat"+savematDate+".mat");
			//AssetDatabase.CreateAsset (materialNew,"Assets/Resources/combineMat.mat");
			_newmeshobj.GetComponent<MeshRenderer>().sharedMaterial = materialNew; 
			CombineInstance[] combine = new CombineInstance[arrTrr .Length];     
			for (int i = 0; i < arrTrr.Length; i++) {
				
					Rect rect = _rects [i];  
				Mesh tmpMesh=new Mesh();
				if (arrTrr [i].GetComponent<MeshFilter > ()) {
					tmpMesh = arrTrr [i].GetComponent<MeshFilter > ().mesh;

					Vector2[] uvs = new Vector2[tmpMesh.uv.Length];  
					//把网格的uv根据贴图的rect刷一遍  
					for (int j = 0; j < uvs.Length; j++) {  
						uvs [j].x = rect.x + tmpMesh.uv [j].x * rect.width;  
						uvs [j].y = rect.y + tmpMesh.uv [j].y * rect.height;  
					}  
					tmpMesh.uv = uvs; 
				} //else {
//					for (int j = 0; j < uvs.Length; j++) {  
//						uvs [j].x = rect.x + tmpMesh.uv [j].x * rect.width;  
//						uvs [j].y = rect.y + tmpMesh.uv [j].y * rect.height;  
//					} 
//					tmpMesh.uv = uvs;  
			//	}
					combine [i].mesh = tmpMesh;
					combine [i].transform = arrTrr [i].transform.localToWorldMatrix;  
					arrTrr [i].gameObject.SetActive (false);  
				
			}

			//合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
			_newmeshobj.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, true);  
			_newmeshobj.gameObject.SetActive(true);    


		} else {
			Debug.LogWarning ("run first！");
		}
	}
	/// 
	/// 

	/// 
	/// 
	void saveprefab(GameObject _newmeshobj)
	{
		Debug.Log ("save pfb" + _newmeshobj);
		Debug.Log  ("save pfb" + "Assets/savePrefab"+DateTime .Now.ToString("MM-dd HH-mm")+".prefab");
		//if (_newmeshobj) {
		if(GameObject.Find ("CombinedMesh")){
			Debug.Log  ("saving");
			string savepathPref = "Assets/Resources/" + main.savefiledate;
			savepathPref +="/savePrefab"+DateTime.Now.ToString("HH-mm")+".prefab";
			//string savefilepath="Assets/savePrefab"+DateTime.Now.ToString("MM-dd HH-mm")+".prefab";
			PrefabUtility.CreatePrefab (savepathPref, _newmeshobj, ReplacePrefabOptions.ReplaceNameBased);
		}else{
			Debug.LogWarning ("obj not found,run first");

		}
	}
	void savemesh(GameObject _newmeshobj)
	{
		//if (_newmeshobj) {
		if(GameObject.Find ("CombinedMesh")){
			//////////////////////////////保存一个mesh
			string savepathMesh="Assets/Resources/"+main.savefiledate;
			savepathMesh+="/saveMesh"+DateTime.Now.ToString("HH-mm")+".asset";

			if (_newmeshobj.GetComponent<MeshFilter> ().mesh.name!="CombinedMesh") {
				Debug.Log ("the mesh is saved");
			} else {
				Mesh m1 = _newmeshobj.GetComponent<MeshFilter> ().mesh;  
				AssetDatabase.CreateAsset (m1, savepathMesh);  
			}
			//////////////////////////////////end 保存一个mesh
		}else{
			Debug.LogWarning ("obj not found,run first");

		}
	}


}
