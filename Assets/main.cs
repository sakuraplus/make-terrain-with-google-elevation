using UnityEngine;
using System.Collections;
using System .Xml ;


using System;
using System.Text;
using System.IO;



public class main : MonoBehaviour {

    GameObject terrmanager;//= new GameObject();
   // GameObject[] arrObj;// = new GameObject[9];
    GameObject[] arrTrr;//= new GameObject[9];
	[Range (-90,90)]
	public   float lat = 30;			//起点纬度，北极90，南极-90
	[Range (-180,180)]
	public   float lng = 70;			//起点经度，英国东方为正，应该西方为负
	[Range (-90,90)]
	public   float endlat = 20;			//终点纬度
	[Range (-180,180)]
	public   float endlng = 90;			//终点经度

	public Vector2 size = new Vector2 (100, 100);
	//public   float distancelat = 2.0f;	//距离，根据起始点经纬度计算 起始点距离外经纬度，
	//  public   float distancelng = 2.0f;	//用于计算终点经纬度，算法有问题，先不用这个了
	public Vector2 segment=new Vector2(5,5);//每块地图分段数

	public Material matTrr;			//地形预设材质

//    void OnCollisionEnter (Collision   theCollision)
//	{
//        Debug.Log("collision---"+theCollision.collider.name );
//		
//	}
//
//
//    void OnTriggerEnter(Collider collider)
//    {
//        //进入触发器执行的代码
//        Debug.Log("collider--"+collider.name);
//    }

    const float earthR = 100;//地球半径

    

    void Start () {

  	//	Debug.Log("纬度--");
        GameObject terrmanager = new GameObject();
       //	arrObj = new GameObject[9];
      	arrTrr = new GameObject[9];

//        for(int i=0;i<9;i++)
//        {
//            arrObj[i] = new GameObject();
//            arrObj[i].name = "gameobjTrr" + i;           
//        }

        terrmanager.name = "TRRMAG";



      //  float steplat = (float)Math.Abs(distancelat * 360 / (2 * Math.PI *Math.Cos(lat) * earthR));
     //   float steplng = (float)Math.Abs(distancelng * 360 / (2 * Math.PI *  earthR));
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



//		lat+-=steplat;
//		lng+=steplng;
		///////////////////////////////
//		arrObj[0].AddComponent<drawJterrain>().initTrr(lat + steplat, lng-steplng, lat , lng , "Trr00",segment);
//        arrTrr[0] = GameObject.Find("Trr00");
//        arrObj[1].AddComponent<drawJterrain>().initTrr(lat + steplat, lng            , lat , lng + steplng, "Trr01",segment);
//        arrTrr[1] = GameObject.Find("Trr01");
//        arrObj[2].AddComponent<drawJterrain>().initTrr(steplat + lat, lng+steplng, lat, lng + 2*steplng, "Trr02",segment);
//        arrTrr[2] = GameObject.Find("Trr02");
//
//		arrObj[3].AddComponent<drawJterrain>().initTrr(lat, lng-steplng, lat-steplat , lng ,  "Trr10",segment);
//        arrTrr[3] = GameObject.Find("Trr10");
//		arrObj[4].AddComponent<drawJterrain>().initTrr(lat, lng            , lat-steplat , lng +steplng,  "Trr11",segment);
//        arrTrr[4] = GameObject.Find("Trr11");
//		arrObj[5].AddComponent<drawJterrain>().initTrr(lat, lng+steplng, lat-steplat , lng +2*steplng,  "Trr12",segment);
//        arrTrr[5] = GameObject.Find("Trr12");
//
//
//		arrObj[6].AddComponent<drawJterrain>().initTrr(lat-steplat, lng-steplng, lat-2*steplat , lng , "Trr20",segment);
//        arrTrr[6] = GameObject.Find("Trr20");
//		arrObj[7].AddComponent<drawJterrain>().initTrr(lat-steplat, lng            , lat-2*steplat , lng+steplng , "Trr21",segment);
//        arrTrr[7] = GameObject.Find("Trr21");
//		arrObj[8].AddComponent<drawJterrain>().initTrr(lat-steplat, lng+steplng, lat-2*steplat , lng+2*steplng ,  "Trr22",segment);
//        arrTrr[8] = GameObject.Find("Trr22");
//
//
//        arrTrr[0].transform.Translate(new Vector3(-100, -50, -100));
//        arrTrr[1].transform.Translate(new Vector3(0     , -50, -100));
//        arrTrr[2].transform.Translate(new Vector3(100 , -50, -100));
//        arrTrr[3].transform.Translate(new Vector3(-100, -50, 0));
//        arrTrr[4].transform.Translate(new Vector3(0     , -50, 0));
//        arrTrr[5].transform.Translate(new Vector3(100 , -50, 0));
//        arrTrr[6].transform.Translate(new Vector3(-100, -50, 100));
//        arrTrr[7].transform.Translate(new Vector3(     0, -50 ,100));
//        arrTrr[8].transform.Translate(new Vector3(100 , -50, 100));
//		////////////////////////////////////////


    }

}