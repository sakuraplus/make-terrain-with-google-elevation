using UnityEngine;  
//using UnityEditor;  
using System.Collections;  

using System;
using System.Text;
using System.IO;


public class main : MonoBehaviour {

    GameObject terrmanager;//= new GameObject();
  	public  GameObject[] arrTrr;//= new GameObject[9];

	[Header("latitude and longitude of the northwest")]
	[Range (-90,90)]
	public   float lat = 30;			//起点纬度，北极90，南极-90
	[Range (-180,180)]
	public   float lng = 70;			//起点经度，英国东方为正，应该西方为负

	[Header ("latitude and longitude of the southeast")]
	[Range (-90,90)]
	public   float endlat = 20;			//终点纬度
	[Range (-180,180)]
	public   float endlng = 90;			//终点经度



	[SerializeField,HeaderAttribute ("Default material of each block")]
	//[HideInInspector]
	public Material matTrr;	
	//地形预设材质

	[SerializeField,Header ("API KEY of google elevation service")]
	[Tooltip("Get a ELE KEY at developers.google.com/maps/documentation/elevation")]
	string  googleELEAPIKey="";
	[SerializeField,Header ("API KEY of google staticmap service")]
	string  googleSTMAPIKey="";
	[Space(20)]
	public static string ELEAPIkey;
	public static string STMAPIkey;
	//https://developers.google.com/maps/documentation/elevation/start?

	[SerializeField,HeaderAttribute ("segment one mesh block in  lng,lat")]
	public Vector2 segment=new Vector2(5,5);//每块地图分段数

	[Header( "size of the mesh in lat")]
	public float sizemesh=100;

//	[SerializeField,  HeaderAttribute ("size of the mesh")]
	[HideInInspector]
	public  Vector3 size = new Vector3 (100, 100,1);
	[SerializeField,Header("the addition of real height data")]
	[Tooltip("1 means the real scale")]
	[Range (0.01f,1000f)]
	float heightScale=1f;

	[HideInInspector ]
	public GameObject _newmeshobj;//=new GameObject ();

    const float earthR = 6371000;//地球半径
	[SerializeField]
	bool _havelicense=false;

	public static  string savefiledate;
	public static  int NumComplete;
	public static  int NumError;
    void Start () {
		NumComplete = 0;
		NumError = 0;
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

	public void Trimlatlng()
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
		} else if(Math.Abs (endlng - lng) ==180){
			if (lat > endlat) {
				vecnorthwest.x = lng;
				vecsoutheast.x = endlng;
			} else {
				vecnorthwest.x = endlng;
				vecsoutheast.x = lng;
			}
		}else {
			//内角跨+-180度，经度为负的为东侧
			vecnorthwest.x = Mathf.Max  (lng, endlng);
			vecsoutheast.x = Mathf.Min  (lng, endlng);
		}
		lat = vecnorthwest.y;
		lng = vecnorthwest.x;
		endlat = vecsoutheast.y;
		endlng = vecsoutheast.x;
		print ("trim" + lat + "," + lng + "|" + endlat + "," + endlng);
	}

	void makeTrr()
	{
		savefiledate=DateTime.Now.ToString("yyyy-MM-dd HH-mm");
		print ("savefiledate="+savefiledate);
		ELEAPIkey = googleELEAPIKey;
		STMAPIkey = googleSTMAPIKey;
		if (ELEAPIkey.Length < 1) {

			Debug.LogWarning ("you need ele key");
			return;
		}
		if ((lat == endlat) || (lng == endlng)) {
			Debug.LogWarning ("incorrect geographical coordinate");
			return;
		}
		if (lat > 85 || lat < -85 || endlat > 85 || endlat < -85) {
			Debug.LogWarning ("you may not get the right map texture above the +-85 latitude");
		}

		terrmanager = new GameObject();
		arrTrr = new GameObject[9];

		terrmanager.name = "TRRMAG";

	
		Trimlatlng ();//处理输入的经纬度信息，保证为西北，东南两点

		//每个分块纬度差
		float	steplat=(endlat-lat)/3; //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplng;
		if (Math.Abs (endlng - lng) >= 180) {
			//如果=180则认为lat，lng为西北点
			steplng = (360 + endlng - lng) / 3;
		} else {
			steplng = (endlng - lng) / 3;
		}

		size = calcMeshSize (sizemesh);//以纬度方向size y计算经度方向距离x


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

		arrTrr[0].transform.Translate(new Vector3(-1*size.x, -50, size.z));
		arrTrr[1].transform.Translate(new Vector3(-1*size.x, -50, 0));
		arrTrr[2].transform.Translate(new Vector3(-1*size.x , -50, -1*size.z));
		arrTrr[3].transform.Translate(new Vector3(0		, -50, size.z));
		arrTrr[4].transform.Translate(new Vector3(0		, -50, 0));
		arrTrr[5].transform.Translate(new Vector3(0		 , -50,-1*size.z));
		arrTrr[6].transform.Translate(new Vector3(size.x	, -50, size.z));
		arrTrr[7].transform.Translate(new Vector3(size.x	, -50 ,0));
		arrTrr[8].transform.Translate(new Vector3(size.x	 , -50,-1*size.z));


		 _newmeshobj=new GameObject ();

	}


	public Vector3 calcMeshSize(float sizelat)
	{
		Vector3 size;
		size.z = sizelat;
		float	steplatall=Mathf.Abs (endlat-lat); //(float)Math.Floor(steplat*10)/10;
		//每个分块经度差
		//经度差绝对值>180时，取endlng+360计算step.计算后经度超过180的部分在索取数据时处理
		float	steplngall;
		if (Math.Abs (endlng - lng) >= 180) {
			steplngall = (360 + endlng - lng) ;
		} else {
			steplngall = (endlng - lng) ;
		}

		float ttt=(endlat+lat)/2;//区域的平均纬度
		ttt =Mathf.Deg2Rad*ttt;// 角度转弧度=Mathf.PI * ttt / 180;//
		//print ("endlat-lat)/2=" + ttt+ "   cos (ttt)="+Mathf.Cos (ttt)+"  ///"+Mathf.Acos(ttt));
		ttt=  Mathf.Abs (Mathf.Cos(ttt ));//当前纬度下，1纬度与1经度之间的距离比
		print ("Math.Abs (Math.Cos(ttt ))" + ttt);
		size.x =size.z * ttt * Mathf.Abs (steplngall / steplatall);//根据当前纬度下跨越的纬度与跨越的经度距离的比例关系，求lng方向的mesh尺寸

		float distancelat = 2 * Mathf.PI * earthR * steplatall / 360;//计算纬度方向实际距离
		float _scale =3* size.z / distancelat;//单位实际距离对应的mesh大小
	
		size.y=_scale*heightScale ;
		print("distancelat="+distancelat+"  _add="+_scale+"  sizey="+size.y);
		return size;
		//print("steplat=" + steplat + "  steplng=" + steplng+" size="+size );//steplat0.5729578steplng3.71444
	}



}
