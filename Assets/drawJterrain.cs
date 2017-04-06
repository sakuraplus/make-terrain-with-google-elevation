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
	string StrWwwData;
    public float lat = 40.00f;//+-90
    public float lng = 116.00f;//+-180
     float steplat = 116.00f;//+-180
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



    private Vector2[] uvs;
	private ArrayList   strfile=new ArrayList() ;
	private int[] triangles;
	
	//生成信息
	private Vector2 size;//长宽
	private float minHeight = -10;
	private float maxHeight = 10;    

	private float unitH;

	//面片mesh
	private GameObject terrain;

    public string  test()
    {
       return terrain.name;
    }

    public void initTrr(float _lat,float _lng, float endlat, float endlng, string _Trrname)
    {
        lat = _lat;
        lng = _lng;
        Trrname = _Trrname;


        int leng = ((int)segment.x + 1) * ((int)segment.y + 1);
        print(_lat+ "//" + _lng+ "//" + endlat+"//"+ endlng+"---"+_Trrname);
        vertives = new Vector3[leng];
        vtest = new Vector3[leng];////////////////////////////


        northeastlat = lat;// = -90;//+-90
        northeastlng = lng;// = -180;//+-180
        southwestlat = endlat;// = -90;//+-90
        southwestlng = endlng;//= -180;//+-180
        steplat = (southwestlat - northeastlat) / segment.x;


        Init(100, 100, (uint)segment.x, (uint)segment.y, -10, 10);
        GetUV();
        GetTriangles();

        StartCoroutine(LoadJson(lat, lng));

    }
  

  

    public IEnumerator LoadJson(float lat,float lng)
	{  
	   if (indVertives >= vertives.Length)
		 {
		   /////////////////
			Debug.Log("break!!!!!!!");
			string sttt = "";


            DrawMesh();
			yield break;
		 }

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //path//locations
        ipaddress +=lat +","+northeastlng +"|";
        ipaddress += lat  +","+southwestlng ;
        ipaddress += "&samples=" + (segment.y+1);
        ipaddress +="&key=AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk";
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

                for (int i=0; i < GoogleJsonData["results"].Count ; i++)
                {
                    int tempsss = +UnityEngine.Random.Range(0, 10);
                  vertives[indVertives + i] = new Vector3(i*100/segment.x, float.Parse(GoogleJsonData["results"][i]["elevation"].ToString()) / 100, (indVertives / GoogleJsonData["results"].Count) * 100/segment.y);
                    vtest[indVertives + i] = new Vector3(float.Parse(GoogleJsonData["results"][i]["location"]["lat"].ToString()), float.Parse(GoogleJsonData["results"][i]["elevation"].ToString()) / 100, float.Parse(GoogleJsonData["results"][i]["location"]["lng"].ToString()));

                }
                indVertives += GoogleJsonData["results"].Count;


                lat += steplat;
           
                StartCoroutine(LoadJson(lat,lng));  
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
        print("init--segmentX" + segmentX);
		size = new Vector2(width, height);
		maxHeight = max;
		minHeight = min;
		unitH = maxHeight - minHeight;
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


//        if (diffuseMap == null)
//        {
//            diffuseMap = new Material(Shader.Find("Diffuse"));
//        }
//        terrain.GetComponent<Renderer>().material = diffuseMap;

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
        terrain.AddComponent<MeshCollider>();
        terrain.GetComponent<MeshCollider>().sharedMesh = mesh ;
        terrain.GetComponent<MeshCollider>().convex = true;
    }


	//
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
		int sum = Mathf.FloorToInt(segment.x * segment.y * 6);
		triangles = new int[sum];
		uint index = 0;
		for (int i = 0; i < segment.y; i++)
		{
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
			}
		}
		return triangles;
	}
	/// 
	///////////////////////////
	
	
	
	
	/////////////////////////
	// Update is called once per frame
	
}