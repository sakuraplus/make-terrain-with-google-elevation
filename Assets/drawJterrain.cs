using UnityEngine;  
using UnityEditor;  
using System.Collections;  
using LitJson;  



using System;
using System.Text;
using System.IO;



public class drawJterrain : MonoBehaviour {
	
	GameObject Player ;
	
	string  ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?locations="; 
	public string googleKey ="AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk";//需要自己注册！！
	string StrWwwData;
//		public float lat = 40.00f;//+-90
//		public float lng = 116.00f;//+-180
    float steplat ;//= 116.00f;//+-180
    public string Trrname;

    public  float northeastlat;// = -90;//+-90
    public  float northeastlng;// = -180;//+-180
    public  float southwestlat;// = -90;//+-90
    public  float southwestlng;//= -180;//+-180

	Vector2 segment=new Vector2(3,3);//每块分段数量
	
    int indVertives=0;

    public Material diffuseMap;

    private Vector3[] vertives;
    private Vector3[] vtest;///////////////////////////


	float sizelat=100;
	float  sizelng=100;


    private Vector2[] uvs;
	//	private ArrayList   strfile=new ArrayList() ;
	private int[] triangles;
	
	//生成信息
	//	private Vector2 size;//长宽
	//	private float minHeight = -10;
	//	private float maxHeight = 10;    

	//	private float unitH;


	private GameObject terrain;

    public string  test()
    {
       return terrain.name;
    }

	public void initTrr(float _northeastlat,float _northeastlng, float _southwestlat, float _southwestlng, string _Trrname,Vector2 _segment,Vector2 _size,Material _mat = null)
    {
		diffuseMap = _mat;

		sizelat = _size.y;
		sizelng = _size.x;

        Trrname = _Trrname;

		segment=_segment;
        int leng = ((int)segment.x + 1) * ((int)segment.y + 1);

		//////////////测试倒序
		//indVertives=leng-1;
		/////////////////////////测试倒序

        vertives = new Vector3[leng];//用于存每个点的坐标


		if (_northeastlng > 180) {
			_northeastlng -=360 ;
		}
		if (_southwestlng > 180) {
			_southwestlng -=360;
		}
        northeastlat = _northeastlat;// = -90;//+-90 东北角纬度
        northeastlng = _northeastlng;// = -180;//+-180 东北角经度
        southwestlat = _southwestlat;// = -90;//+-90 西南角纬度
        southwestlng = _southwestlng;//= -180;//+-180 西南角经度
        steplat =- (southwestlat - northeastlat) / segment.x;//每段跨越的纬度
		//z正方向为北


        Init(100, 100, (uint)segment.x, (uint)segment.y, -10, 10);//mesh宽度
        GetUV();
        GetTriangles();

        StartCoroutine(LoadJson(northeastlat));

    }
  

  

    public IEnumerator LoadJson(float lat)
	{  
		/////////测试倒序  if (indVertives < 0)
	   if (indVertives >= vertives.Length)		  
		 {
		   /////////////////
			Debug.Log("Data complete!!!!!!!");
            DrawMesh();
			yield break;
		 }

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
        ipaddress +=lat +","+northeastlng +"|";
        ipaddress += lat  +","+southwestlng ;//获取同一纬度下，东西经度之间的数据
        ipaddress += "&samples=" + (segment.y+1)+"&key=";
        ipaddress +=googleKey;//需要自己注册！！
		print(ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.Log("error :" + www_data.error );
			StrWwwData =  "error :" + www_data.error;    
		}    
		else    
		{    
		try{  
				StrWwwData = www_data.text;    
				JsonData GoogleJsonData = JsonMapper.ToObject(StrWwwData);
				//for (int i=GoogleJsonData["results"].Count-1; i >0; i--)/////测试倒序
				for (int i=0; i < GoogleJsonData["results"].Count ; i++)		
                {

		

					 //	 vertives[indVertives -GoogleJsonData["results"].Count + i]  ///测试倒序
					vertives[indVertives + i]= new Vector3(i*sizelat /segment.x, float.Parse(GoogleJsonData["results"][i]["elevation"].ToString()) / 100, (indVertives / GoogleJsonData["results"].Count) * sizelng/segment.y);
					 //100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
		              //Vector3（x坐标，google数据中的高度，z坐标，xz可能需要交换？？？或者倒序？）
					  //现在东南西北好像反了ò。ó？

                }
               // indVertives -= GoogleJsonData["results"].Count;/////////测试倒序
				indVertives += GoogleJsonData["results"].Count;/////////测试倒序

                lat += steplat;
           
                StartCoroutine(LoadJson(lat));  //获取下一纬度，东西经度之间的数据
				StrWwwData = "";  				
			}  
			catch (Exception ex)  
			{  
				Debug.Log(ex.ToString());  
			}  
			
			finally  	{}  
			
		}//end else		
	}//end LoadFile

  

	///////////////////////////
	/// 
	private void Init(float width, float height, uint segmentX, uint segmentY, int min, int max)
	{
   //     print("init--segmentX" + segmentX);

		segment = new Vector2(segmentX, segmentY);
		if (terrain != null)
		{
			Destroy(terrain);
		}
		terrain = new GameObject();
        terrain.name = Trrname;// "cubeTTT";
	}

  //
    private void DrawMesh()
	{
		Mesh mesh = terrain.AddComponent<MeshFilter>().mesh;
		terrain.AddComponent<MeshRenderer>();


        if (diffuseMap == null)
        {
			diffuseMap = new Material(Shader.Find("Standard"));
        }
        terrain.GetComponent<Renderer>().material = diffuseMap;

        //给mesh 赋值
        mesh.Clear();
		mesh.vertices = vertives;//,pos);
		mesh.uv = uvs;
		mesh.triangles = triangles;
		//重置法线
		mesh.RecalculateNormals();
		//重置范围
		mesh.RecalculateBounds();

        ////////////////////////
//        terrain.AddComponent<MeshCollider>();
//        terrain.GetComponent<MeshCollider>().sharedMesh = mesh ;
//        terrain.GetComponent<MeshCollider>().convex = true;
    }


	//设定每个顶点的uv
    private Vector2[] GetUV()
	{
        int sum = vertives.Length;
		uvs = new Vector2[sum];
		float u = 1.0F / segment.x;
		float v = 1.0F / segment.y;
		uint index = 0;
		for (int i = 0; i < segment.y + 1; i++)
		{
			for (int j = 0; j < segment.x + 1; j++)
			{
				uvs[index] = new Vector2(j * u, i * v);
				index++;
			}
		}
		return uvs;
	}
	

	private int[] GetTriangles()
	{
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);//每格两个三角形，6个顶点
		triangles = new int[sum];
		uint index = 0;
		for (int i = 0; i < segment.y; i++)
		{
			//y对应z方向
			for (int j = 0; j < segment.x; j++)
			{
				int role = Mathf.FloorToInt(segment.x) + 1;
				int self = j +( i*role);                
				int next = j + ((i+1) * role);
				triangles[index] = self;
				triangles[index + 1] = next + 1;
				triangles[index + 2] = self + 1;
				triangles[index + 3] = self;
				triangles[index + 4] = next;
				triangles[index + 5] = next + 1;
				index += 6;
				//
			}
		}
		return triangles;
	}
	/// 

	
}