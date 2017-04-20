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
		// show the default inspector stuff for this component
		DrawDefaultInspector();

		// get a reference to the GameManager script on this target gameObject
		main _main = (main)target;

		GUILayout.Label ("mesh size="+_main.size.ToString () );

		// add a custom button to the Inspector component
		if(GUILayout.Button("combine meshs"))
		{
			meshcombine (_main._newmeshobj,_main.arrTrr );

		//	_main.meshcombine ();//EditorCall ();//editor 不调用协程
		}
		if(GUILayout.Button("save as prefab"))
		{
			savemesh   (_main._newmeshobj);
			saveprefab (_main._newmeshobj );
			//_main.saveprefab  ();
		}
		if(GUILayout.Button("save as mesh only"))
		{

			savemesh   (_main._newmeshobj);
			//_main.savemesh   ();
		}
		GUILayout.Label ("test" );
		if(GUILayout.Button("test"))
		{
			//_main.Trimlatlng ();
			//_main.size=_main.calcMeshSize(_main.sizemesh);
			Debug.Log("test"+  main.savefiledate); 
			Texture2D _texture = Resources.Load ("combineTex201704202126") as Texture2D;
			Debug.Log("1 _texture="+_texture );
			_texture = Resources.Load ("2017-04-20 21-26/combineTex201704202126") as Texture2D;
			Debug.Log("2 _texture="+_texture );
			_texture = Resources.Load ("combineTex201704202126.png") as Texture2D;
			Debug.Log("3 _texture="+_texture );
			_texture = Resources.Load ("Resources/2017-04-20 21-26/combineTex201704202126.png") as Texture2D;
			Debug.Log("4 _texture="+_texture );
			//Debug.Log("test"+  _main.savefiledate); //

		}
	}


	/// 
	/// 
	/// 

	void meshcombine(GameObject _newmeshobj,GameObject[] arrTrr)
	{
		Debug.Log ("num=" + main.NumComplete);
		if (main.NumComplete < arrTrr.Length ) {
			Debug.LogWarning ("wait");
			return;
		}
		Debug.Log (GameObject.Find ("TRRMAG"));
		if (GameObject.Find ("TRRMAG")) {
			Debug.Log ("exist");
			//MeshRenderer[] _meshrenders=new MeshRenderer[arrTrr.Length ] ;
			Material[] _materials = new Material[arrTrr.Length ];
			Texture2D[] _textures = new Texture2D[arrTrr.Length];
			for (int i = 0; i < arrTrr.Length; i++) {
				_materials [i] = arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial;

				Texture2D tx = _materials[i].GetTexture("_MainTex") as Texture2D; 
				Texture2D tx2D = new Texture2D(tx.width, tx.height, TextureFormat.ARGB32, false);  
				var tmp = tx.GetPixels (0, 0, tx.width, tx.height);
				tx2D.SetPixels(tmp);  
				tx2D.Apply();  
				_textures[i] = tx2D;  
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


			Material materialNew = new Material(_materials[0].shader);  
			materialNew.CopyPropertiesFromMaterial(_materials[0]);  

			materialNew.SetTexture("_MainTex", _texture); 
			AssetDatabase.CreateAsset (materialNew,filepathMaterial +"/combineMat"+savematDate+".mat");
			//AssetDatabase.CreateAsset (materialNew,"Assets/Resources/combineMat.mat");
			_newmeshobj.GetComponent<MeshRenderer>().sharedMaterial = materialNew; 
			CombineInstance[] combine = new CombineInstance[arrTrr .Length];     
			for (int i = 0; i < arrTrr.Length; i++) {

				Rect rect = _rects[i];  
				Mesh tmpMesh = arrTrr [i].GetComponent<MeshFilter >().mesh ;

				Vector2[] uvs = new Vector2[tmpMesh.uv.Length];  
				//把网格的uv根据贴图的rect刷一遍  
				for (int j = 0; j < uvs.Length; j++)  
				{  
					uvs[j].x = rect.x + tmpMesh.uv[j].x * rect.width;  
					uvs[j].y = rect.y + tmpMesh.uv[j].y * rect.height;  
				}  
				tmpMesh.uv = uvs;  

				combine [i].mesh = tmpMesh;
				combine[i].transform = arrTrr[i].transform.localToWorldMatrix;  
				arrTrr[i].gameObject.SetActive(false);  
			}
			//Debug.Log ("combine--"+combine[0]);


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
			//string baseResultFolder="Assets/savemesh";
			//string  dateStr = DateTime.Now.ToString("yyyy-MM-dd HH-mm");
			//baseResultFolder += dateStr;
//			if (! Directory.Exists(baseResultFolder)) 
//			{
//				Directory.CreateDirectory(baseResultFolder);
//			}
			//string strfilename=baseResultFolder+"/aaa" +".asset";
			Mesh m1 = _newmeshobj . GetComponent<MeshFilter>().mesh;  
			AssetDatabase.CreateAsset(m1, savepathMesh);  
			//////////////////////////////////end 保存一个mesh
		}else{
			Debug.LogWarning ("obj not found,run first");

		}
	}


}
