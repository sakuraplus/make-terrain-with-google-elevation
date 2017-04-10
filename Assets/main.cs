using UnityEngine;
using System.Collections;
using System .Xml ;


using System;
using System.Text;
using System.IO;



public class main : MonoBehaviour {

    GameObject terrmanager;//= new GameObject();
    GameObject[] arrObj;// = new GameObject[9];
    GameObject[] arrTrr;//= new GameObject[9];
	[Range (-180,180)]
	public   float lat = 30;			//起点纬度，北极180，南极-180
	[Range (-180,180)]
	public   float lng = 70;			//起点经度，英国东方为正，应该西方为负
	[Range (-180,180)]
	public   float endlat = 20;			//终点纬度
	[Range (-180,180)]
	public   float endlng = 90;			//终点经度
	//public   float distancelat = 2.0f;	//距离，根据起始点经纬度计算 起始点距离外经纬度，
	//  public   float distancelng = 2.0f;	//用于计算终点经纬度，算法有问题，先不用这个了
	public Vector2 segment=new Vector2(5,5);//每块地图分段数

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
       	arrObj = new GameObject[9];
      	arrTrr = new GameObject[9];

        for(int i=0;i<9;i++)
        {
            arrObj[i] = new GameObject();
            arrObj[i].name = "gameobjTrr" + i;           
        }

        terrmanager.name = "TRRMAG";



      //  float steplat = (float)Math.Abs(distancelat * 360 / (2 * Math.PI *Math.Cos(lat) * earthR));
     //   float steplng = (float)Math.Abs(distancelng * 360 / (2 * Math.PI *  earthR));
		float	steplat=(endlat-lat)/3; //(float)Math.Floor(steplat*10)/10;
		float	steplng=(endlng-lng)/3;// (float)Math.Floor(steplng*10)/10;
		//计算每个地图块终点经纬度
        print("steplat=" + steplat + "  steplng=" + steplng);//steplat0.5729578steplng3.71444




		////////////////////////////////
		//起点为左上块
		//可能需要修改，z为经度，x为纬度
		arrObj[0].AddComponent<drawJterrain>().initTrr(lat					,lng				,lat+steplat		,lng+steplng		, "Trr00",segment);
        arrTrr[0] = GameObject.Find("Trr00");
        arrObj[1].AddComponent<drawJterrain>().initTrr(lat+steplat		,lng		        ,lat+steplat*2	,lng+steplng		, "Trr01",segment);
        arrTrr[1] = GameObject.Find("Trr01");
        arrObj[2].AddComponent<drawJterrain>().initTrr(lat+steplat*2 ,lng				,lat+steplat*3	,lng+steplng		, "Trr02",segment);
        arrTrr[2] = GameObject.Find("Trr02");

		arrObj[3].AddComponent<drawJterrain>().initTrr(lat					, lng+steplng	 , lat+steplat		, lng+steplng*2 ,  "Trr10",segment);
        arrTrr[3] = GameObject.Find("Trr10");
		arrObj[4].AddComponent<drawJterrain>().initTrr(lat+steplat		, lng+steplng	 , lat+steplat*2	, lng +steplng*2,  "Trr11",segment);
        arrTrr[4] = GameObject.Find("Trr11");
		arrObj[5].AddComponent<drawJterrain>().initTrr(lat+steplat*2 , lng+steplng	 , lat+steplat*3	, lng +steplng*2,  "Trr12",segment);
        arrTrr[5] = GameObject.Find("Trr12");


		arrObj[6].AddComponent<drawJterrain>().initTrr(lat				 	, lng+steplng*2, lat+steplat	, lng+steplng*3 , "Trr20",segment);
        arrTrr[6] = GameObject.Find("Trr20");
		arrObj[7].AddComponent<drawJterrain>().initTrr(lat+steplat		, lng+steplng*2, lat+steplat*2, lng+steplng*3, "Trr21",segment);
        arrTrr[7] = GameObject.Find("Trr21");
		arrObj[8].AddComponent<drawJterrain>().initTrr(lat+steplat*2 , lng+steplng*2, lat+steplat*3, lng+steplng*3,  "Trr22",segment);
        arrTrr[8] = GameObject.Find("Trr22");


        arrTrr[0].transform.Translate(new Vector3(-100, -50, 100));
        arrTrr[1].transform.Translate(new Vector3(-100, -50, 0));
        arrTrr[2].transform.Translate(new Vector3(-100 , -50, -100));
        arrTrr[3].transform.Translate(new Vector3(0		, -50, 100));
        arrTrr[4].transform.Translate(new Vector3(0     , -50, 0));
        arrTrr[5].transform.Translate(new Vector3(0		 , -50,-100));
        arrTrr[6].transform.Translate(new Vector3(100, -50, 100));
        arrTrr[7].transform.Translate(new Vector3( 100, -50 ,0));
        arrTrr[8].transform.Translate(new Vector3(100 , -50,-100));


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