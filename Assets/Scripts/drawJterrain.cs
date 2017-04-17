using UnityEngine;  
using UnityEditor;  
using System.Collections;  

using System;
using System.Text;
using System.IO;

[Serializable]
public class JsonMapData{
	public MapRes[] results;
	public string status;
}

[Serializable]
public class MapRes{
	public double elevation;
	public MapLocation location;
	public double resolution;
}

[Serializable]
public class MapLocation{
	public double lat;
	public double lng;
}
	
public class drawJterrain : MonoBehaviour {
	
	GameObject Player ;

	string  ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?locations="; 
	string googleKey = main.APIkey;// = "AIzaSyD04LHgbiErZTYJMfda2epkG0YeaQHVuEE";//需要自己注册！！
	//;//"AIzaSyApPJ8CP4JxKWIW2vavwdRl6fnDvdcgCLk"
	string StrWwwData;

    float steplat ;//= 116.00f;//+-180
    public string Trrname;

    public  float northwestlat;// = -90;//+-90
    public  float northwestlng;// = -180;//+-180
    public  float southeastlat;// = -90;//+-90
    public  float southeastlng;//= -180;//+-180

	Vector2 segment=new Vector2(3,3);//每块分段数量
	
    int indVertives=0;

    public Material diffuseMap;

    private Vector3[] vertives;
    private Vector3[] vtest;///////////////////////////


	float sizelat=100;
	float  sizelng=100;


    private Vector2[] uvs;
	private int[] triangles;
	

	string tempstr="";//打印测试数据用

	private GameObject terrain;

    public string  test()
    {
       return terrain.name;
    }

	public void initTrr(float _northwestlat,float _northwestlng, float _southeastlat, float _southeastlng, string _Trrname,Vector2 _segment,Vector2 _size, Material _matTrr = null)
    {
		diffuseMap = _matTrr;

		sizelat = _size.y;
		sizelng = _size.x;

        Trrname = _Trrname;

		segment=_segment;
        int leng = ((int)segment.x + 1) * ((int)segment.y + 1);

		//////////////测试倒序
		//indVertives=leng-1;
		/////////////////////////测试倒序

        vertives = new Vector3[leng];//用于存每个点的坐标


		if (_northwestlng > 180) {
			_northwestlng -=360 ;
		}
		if (_southeastlng > 180) {
			_southeastlng -=360;
		}
        northwestlat = _northwestlat;// = -90;//+-90 西北角纬度
        northwestlng = _northwestlng;// = -180;//+-180西北角经度
        southeastlat = _southeastlat;// = -90;//+-90 东南角纬度
        southeastlng = _southeastlng;//= -180;//+-180 东南角经度
        steplat = ( northwestlat-southeastlat ) / segment.y;//每段跨越的纬度
		//z正方向为北
		print (Trrname+"-init-"+northwestlat+","+_northwestlng+"//"+_southeastlat+","+_southeastlng+" step="+steplat);

        Init(100, 100, (uint)segment.x, (uint)segment.y, -10, 10);//mesh宽度
        GetUV();
        GetTriangles();

       // StartCoroutine(LoadJson(southeastlat));//多边形顶点从左south开始
		testVertives();//测试segment xy不同时生成mesh,不读google数据

    }
  

  	void testVertives()
	{
		System.Random rm=new System.Random();
		for (int indVertives = 0; indVertives < vertives.Length; indVertives += ((int)segment.x + 1))
		{
			for (int i = 0; i <(segment.x + 1); i++) {

				vertives [indVertives + i] = 
				new Vector3 (i * sizelat / segment.x,
						rm.Next (2) , 
						(indVertives / (segment.x + 1)) * sizelng / segment.y);
				//100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
				tempstr += vertives [indVertives + i].ToString ();


			}
		}
		print(tempstr);
		DrawMesh();
		StartCoroutine(	loadimg ());
	}


    public IEnumerator LoadJson(float lat)
	{  
		/////////测试倒序  if (indVertives < 0)
	   	if (indVertives >= vertives.Length)		  
		{
		   /////////////////
			Debug.Log(Trrname + "Data complete!!!!!!!"+tempstr );
            DrawMesh();
			yield break;
		}

		ipaddress = "https://maps.googleapis.com/maps/api/elevation/json?path="; //获取json数据,改为XML获取xml数据
        ipaddress +=lat +","+northwestlng +"|";
        ipaddress += lat  +","+southeastlng ;//获取同一纬度下，东西经度之间的数据
        ipaddress += "&samples=" + (segment.x+1)+"&key=";
        ipaddress +=googleKey;//需要自己注册！！
		print(Trrname+"--"+ipaddress);
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  

		StrWwwData = www_data.text;   
		////////////////////////////
		if (www_data.error != null)    
		{    
			Debug.Log("error :"+Trrname +"/"+indVertives +"-" + www_data.error );

			StrWwwData =  "error :" + www_data.error;    
		}    
		else    
		{    
			try{  
				StrWwwData = www_data.text;    
				JsonMapData GoogleJsonData = JsonUtility.FromJson<JsonMapData>(StrWwwData);
				for (int i=0; i < GoogleJsonData.results.Length ; i++)		
                {
					 //	 vertives[indVertives -GoogleJsonData["results"].Count + i]  ///测试倒序
					vertives[indVertives + i]= new Vector3(i*sizelat /segment.x, float.Parse(GoogleJsonData.results[i].elevation.ToString()) 
						/ 100, (indVertives / GoogleJsonData.results.Length) * sizelng/segment.y);
					 //100/x方向分段数=顶点坐标，高度/100=顶点z，为多边形的
					tempstr +=GoogleJsonData.results[i].location.lat.ToString()+","+GoogleJsonData.results[i].location.lng.ToString()+vertives[indVertives + i].ToString ();//测试数据

                }
              
				indVertives =indVertives+(int)segment.x+1;//+= GoogleJsonData["results"].Count;/////////
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

  //获取当前范围的贴图并保存
	IEnumerator loadimg()
	{

		float centerlat = (southeastlat + northwestlat) / 2;
		float centerlng = (southeastlng + northwestlng) / 2;



		float lerplat=Math.Abs(southeastlat - northwestlat);//范围跨越的经度
		float lerplng=Math.Abs(southeastlng - northwestlng);//范围跨越的经度
		int defaultmapsize = 256;//最终获取图片的参考宽度,免费key最大尺寸

		int tempsize =(int)Math.Abs( (defaultmapsize * 360 / 256) / lerplat);//计算保证获取图片最小尺寸在tempsize以上时所需的zoom
		int nextpoweroftwo =(int)Mathf.ClosestPowerOfTwo(tempsize);//计算保证获取图片最小尺寸在tempsize以上时所需的zoom
		int zoommap = (int)Math.Floor (Mathf.Log(nextpoweroftwo ,2));//计算保证获取图片最小尺寸在tempsize以上时所需的zoom

		float mapsize = 256 * Mathf.Pow (2, zoommap);//在当前zoom下，完整地图的大小

		print (Trrname+" tempsize= "+tempsize+"  lerplat= " + lerplat + "  nextpoweroftwo= " + nextpoweroftwo);

		int sizemapx=(int)Math.Abs( lerplat  *mapsize/360);//完整地图等分360份为跨经度1的宽度
		//int sizemapy=(int)Math.Abs( lerplng  *mapsize/360);//512;//lng
		//print (Trrname+" sizemapx= "+sizemapx+" sizemapy=  "+sizemapy);
		////////////////
		// north
		float sinnorthlat=Mathf.Sin(northwestlat *Mathf.PI /180);
		sinnorthlat = Mathf.Min (Mathf.Max (sinnorthlat, -0.99f), 0.99f);
		float pointnorthlat=mapsize *(0.5f - Mathf.Log ((1 + sinnorthlat) / (1 - sinnorthlat)) / (4 * Mathf.PI));
		/// 
		// south
		float sinsouthlat=Mathf.Sin(southeastlat  *Mathf.PI /180);
		sinsouthlat = Mathf.Min (Mathf.Max (sinsouthlat, -0.99f), 0.99f);
		float pointsouthlat=mapsize *(0.5f - Mathf.Log ((1 + sinsouthlat) / (1 - sinsouthlat)) / (4 * Mathf.PI));

		print (Trrname+" north lat= " + pointnorthlat + "  south lat= " + pointsouthlat+"  mapsize= "+mapsize); 
		int sizemapy =(int) Mathf.Abs (pointsouthlat - pointnorthlat);
		/// /////////////////////
		float tempcentery=(pointsouthlat+pointnorthlat )/2;
		float tempc = 4 * Mathf.PI * (0.5f - (tempcentery / mapsize));
		float templog = Mathf.Exp (tempc);
		float sincentery = (templog - 1) / (templog + 1);
		float newcenterlat =Mathf.Asin(sincentery )*180/Mathf.PI ;
		print (Trrname+ " new center  "+newcenterlat );
		/// templog=(1+siny)/(1-siny)
		/// 
		/// //////////////////
		string strmaptype="roadmap";

		string 	ipaddress = "https://maps.googleapis.com/maps/api/staticmap?center="; //获取
		ipaddress+=newcenterlat+","+centerlng+"&zoom="+zoommap;
		ipaddress += "&size=" + sizemapx + "x" + sizemapy + "&maptype="+strmaptype + "&key=";
		ipaddress += "AIzaSyCljEOXoKPrh9x-xAbpVirQN4fKeI1H9mA";

		print (Trrname+"  loadimg  "+ipaddress );
		WWW www_data = new WWW(ipaddress);  
		yield return www_data;  

		if (www_data.error != null) {
			print ("Load img error" + www_data.error);
		}else{
			print ("load "+www_data );
			//text
			Texture2D tex2d = www_data.texture;  
			//将图片保存至缓存路径  
			byte[] bytes = tex2d.EncodeToPNG();  
			//byte[] bytes = texture2D.EncodeToPNG();
			string strfilename="Assets/test"+Trrname +".png";
			File.WriteAllBytes(strfilename, bytes);
		
		}

	}



	///////////////////////////
	/// 
	private void Init(float width, float height, uint segmentX, uint segmentY, int min, int max)
	{
 
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