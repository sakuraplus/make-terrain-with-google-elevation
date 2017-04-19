using UnityEngine;  
using UnityEditor;  
using System.Collections;  

using System;
using System.Text;
using System.IO;


public class main : MonoBehaviour {

    GameObject terrmanager;//= new GameObject();
    GameObject[] arrTrr;//= new GameObject[9];
	[SerializeField,HeaderAttribute ("latitude and longitude of the northwest")]
	[Range (-90,90)]
	public   float lat = 30;			//起点纬度，北极90，南极-90
	[Range (-180,180)]
	public   float lng = 70;			//起点经度，英国东方为正，应该西方为负

	[SerializeField,HeaderAttribute ("latitude and longitude of the southeast")]
	[Range (-90,90)]
	public   float endlat = 20;			//终点纬度
	[Range (-180,180)]
	public   float endlng = 90;			//终点经度

	[SerializeField,HeaderAttribute ("size of the mesh")]
	public Vector2 size = new Vector2 (100, 100);

	[SerializeField,HeaderAttribute ("segment one mesh block in  lng,lat")]
	public Vector2 segment=new Vector2(5,5);//每块地图分段数
	[SerializeField,HeaderAttribute ("Default material of each block")]
	[HideInInspector]
	public Material matTrr;			//地形预设材质
	[SerializeField,HeaderAttribute ("Get a ELE KEY at developers.google.com/maps/documentation/elevation")]
	string  googleELEAPIKey="";
	[SerializeField,HeaderAttribute ("Get a STM KEY at ")]
	string  googleSTMAPIKey="";
	public static string ELEAPIkey;
	public static string STMAPIkey;
	//https://developers.google.com/maps/documentation/elevation/start?
    const float earthR = 100;//地球半径
	[SerializeField]
	bool _havelicense=false;

    void Start () {
		
		StartCoroutine (findLicense ());

    }
	public void EditorCall()
	{
		StartCoroutine (findLicense ());
	}


	IEnumerator findLicense()
	{
		if (_havelicense) {
			print ("do not find license");
			makeTrr ();
			yield break ;
		}	
	 	string 	ipaddress = "https://sakuraplus.github.io/make-terrain-with-google-elevation/"; //获取
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  

		if (www_data.error != null) {
			print ("Load license error" + www_data.error);
		}else{
			string strlicense = www_data.text;
			//print (strlicense);
			strlicense = strlicense.Substring (0, 15);
			byte[] data = System.Text.Encoding.Default.GetBytes(strlicense);//(byte)strlicense;
			string base64str = System.Convert.ToBase64String(data); 
			print ("!!! "+base64str);
			if (base64str == "c2FrdVNha3VEb0hhcHB5") {
				print ("license correct!");
				_havelicense = true;
				makeTrr ();

			} else {
				print ( www_data.text.Substring (0, 144));
				print ("license incorrect");
			}
		}
	
	}

	void Trimlatlng()
	{
		Vector2 vecnorthwest;
		Vector2 vecsoutheast;
		vecnorthwest.y = Mathf.Max (lat, endlat);
		vecsoutheast.y = Mathf.Min (lat, endlat);
		//if (Mathf.Sign (lng) == Mathf.Sign (endlng)) 
		if(Math.Abs (endlng - lng) < 180){
			// 内角不跨+-180度则经度小的为西侧
			vecnorthwest.x = Mathf.Min (lng, endlng);
			vecsoutheast.x = Mathf.Max (lng, endlng);
		} else {
			//内角跨+-180度，经度为负的为东侧
			vecnorthwest.x = Mathf.Max  (lng, endlng);
			vecsoutheast.x = Mathf.Min  (lng, endlng);
		}
		lat = vecnorthwest.y;
		lng = vecnorthwest.x;
		endlat = vecsoutheast.y;
		endlng = vecsoutheast.x;
	}

	void makeTrr()
	{
		ELEAPIkey = googleELEAPIKey;
		STMAPIkey = googleSTMAPIKey;
		if (ELEAPIkey.Length < 1) {

			Debug.LogWarning ("you need ele key");
			return;
		}


		terrmanager = new GameObject();
		arrTrr = new GameObject[9];

		terrmanager.name = "TRRMAG";

		if ((lat == endlat) || (lng == endlng)) {
			Debug.LogWarning ("incorrect geographical coordinate");
			return;
		}
		Trimlatlng ();//处理输入的经纬度信息，保证为西北，东南两点

		//每个分块纬度差
		float	steplat=(endlat-lat)/3; //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplng;
		if (Math.Abs (endlng - lng) > 180) {
			steplng = (360 + endlng - lng) / 3;
		} else {
			steplng = (endlng - lng) / 3;
		}


		//=(endlng-lng)/3;// (float)Math.Floor(steplng*10)/10;
		//计算每个地图块终点经纬度


		float ttt=(endlat+lat)/2;
		ttt = Mathf.PI * ttt / 180;//
		//print ("endlat-lat)/2=" + ttt+ "   cos (ttt)="+Mathf.Cos (ttt)+"  ///"+Mathf.Acos(ttt));
		ttt= size.x * Mathf.Abs (Mathf.Cos(ttt ));
		print ("Math.Abs (Math.Cos(ttt ))" + ttt);
		size.y = ttt * Mathf.Abs (steplng / steplat);;

		print("steplat=" + steplat + "  steplng=" + steplng+" size="+size );//steplat0.5729578steplng3.71444



		////////////////////////////////
		//起点为左上块
		//可能需要修改，z为经度，x为纬度
		terrmanager.AddComponent<drawJterrain>().initTrr(lat,lng,lat+steplat,lng+steplng, "Trr00",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat,lng,lat+steplat*2	,lng+steplng, "Trr01",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat*2 ,lng,lat+steplat*3	,lng+steplng, "Trr02",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat, lng+steplng, lat+steplat, lng+steplng*2 ,  "Trr10",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat, lng+steplng, lat+steplat*2, lng +steplng*2,  "Trr11",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat*2 , lng+steplng, lat+steplat*3, lng +steplng*2,  "Trr12",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat, lng+steplng*2, lat+steplat	, lng+steplng*3 , "Trr20",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat, lng+steplng*2, lat+steplat*2, lng+steplng*3, "Trr21",segment,size,matTrr);
		terrmanager.AddComponent<drawJterrain>().initTrr(lat+steplat*2 , lng+steplng*2, lat+steplat*3, lng+steplng*3,  "Trr22",segment,size,matTrr);



		arrTrr[0] = GameObject.Find("Trr00");
		arrTrr[1] = GameObject.Find("Trr01");
		arrTrr[2] = GameObject.Find("Trr02");
		arrTrr[3] = GameObject.Find("Trr10");
		arrTrr[4] = GameObject.Find("Trr11");
		arrTrr[5] = GameObject.Find("Trr12");
		arrTrr[6] = GameObject.Find("Trr20");
		arrTrr[7] = GameObject.Find("Trr21");
		arrTrr[8] = GameObject.Find("Trr22");

		for(int i=0;i<arrTrr.Length;i++){
			arrTrr [i].transform.parent = terrmanager.transform;
		}

		arrTrr[0].transform.Translate(new Vector3(-1*size.y, -50, size.x));
		arrTrr[1].transform.Translate(new Vector3(-1*size.y, -50, 0));
		arrTrr[2].transform.Translate(new Vector3(-1*size.y , -50, -1*size.x));
		arrTrr[3].transform.Translate(new Vector3(0		, -50, size.x));
		arrTrr[4].transform.Translate(new Vector3(0		, -50, 0));
		arrTrr[5].transform.Translate(new Vector3(0		 , -50,-1*size.x));
		arrTrr[6].transform.Translate(new Vector3(size.y	, -50, size.x));
		arrTrr[7].transform.Translate(new Vector3(size.y	, -50 ,0));
		arrTrr[8].transform.Translate(new Vector3(size.y	 , -50,-1*size.x));




	}
	GameObject _newmeshobj;
	public void meshcombine()
	{
		print (terrmanager);
		print (GameObject.Find ("TRRMAG"));
		if (GameObject.Find ("TRRMAG")) {
			print ("exist");
			//MeshRenderer[] _meshrenders=new MeshRenderer[arrTrr.Length ] ;
			Material[] _materials = new Material[arrTrr.Length ];
			for (int i = 0; i < arrTrr.Length; i++) {
				_materials [i] = arrTrr [i].GetComponent<MeshRenderer > ().sharedMaterial;			
			}
			print ("mat="+_materials [0]);

			//---------------- 合并 Mesh -------------------------  
			//MeshFilter[] meshFilters = new MeshFilter[arrTrr.Length ] ;//
			CombineInstance[] combine = new CombineInstance[arrTrr .Length];     
			for (int i = 0; i < arrTrr.Length; i++) {
				combine[i].mesh = arrTrr [i].GetComponent<MeshFilter >().mesh ;
				combine[i].transform = arrTrr[i].transform.localToWorldMatrix;  
				arrTrr[i].gameObject.SetActive(false);  
			}
			print ("combine--"+combine[0]);
			//获取arrtrr中所有MeshFilter组件  

			//新建一个gameobj  
			_newmeshobj=new GameObject();
			_newmeshobj.name = "CombindeMesh";
			_newmeshobj.AddComponent<MeshRenderer > ();
			_newmeshobj.AddComponent<MeshFilter> ();

			_newmeshobj.GetComponent<MeshFilter>().mesh = new Mesh();   
			//合并Mesh. 第二个false参数, 表示并不合并为一个网格, 而是一个子网格列表  
			_newmeshobj.GetComponent<MeshFilter>().mesh.CombineMeshes(combine, false);  
			_newmeshobj.gameObject.SetActive(true);  

			//为合并后的新Mesh指定材质 ------------------------------  
			_newmeshobj.GetComponent<MeshRenderer>().sharedMaterials = _materials;   

			//////////////////////////////保存一个mesh
			string baseResultFolder="Assets/";
			string  dateStr = DateTime.Now.ToString("yyyy-MM-dd HH-mm");
			baseResultFolder += dateStr;
			if (!Directory.Exists(baseResultFolder)) 
			{
				Directory.CreateDirectory(baseResultFolder);
			}
			string strfilename=baseResultFolder+"/aaa" +".asset";
			Mesh m1 = _newmeshobj . GetComponent<MeshFilter>().mesh;  
			AssetDatabase.CreateAsset(m1, strfilename);  
			//////////////////////////////////end 保存一个mesh

		} else {
			Debug.LogWarning ("run first！");
		}
	}
	public void saveprefab()
	{
		print ("save pfb" + _newmeshobj);
		print ("save pfb" + GameObject.Find("CombindeMesh"));
		if (GameObject.Find("CombindeMesh")) {
			print ("saving");
			PrefabUtility.CreatePrefab ("Assets/xxx2.prefab", GameObject.Find("CombindeMesh"), ReplacePrefabOptions.ReplaceNameBased);
		}
	}

}
