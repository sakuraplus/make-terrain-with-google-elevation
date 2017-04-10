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

	public   float lat = 29;			//起点纬度
    public   float lng = 99;			//起点经度
	public   float distancelat = 2.0f;	//距离，根据起始点经纬度计算 起始点距离外经纬度，
    public   float distancelng = 2.0f;	//用于计算终点经纬度

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

  		Debug.Log("纬度--");
        GameObject terrmanager = new GameObject();
       	arrObj = new GameObject[9];
      	arrTrr = new GameObject[9];

        for(int i=0;i<9;i++)
        {
            arrObj[i] = new GameObject();
            arrObj[i].name = "gameobjTrr" + i;           
        }

        terrmanager.name = "TRRMAG";



        float steplat = (float)Math.Abs(distancelat * 360 / (2 * Math.PI *Math.Cos(lat) * earthR));
        float steplng = (float)Math.Abs(distancelng * 360 / (2 * Math.PI *  earthR));
		steplat= (float)Math.Floor(steplat*10)/10;
		steplng= (float)Math.Floor(steplng*10)/10;
		//计算每个地图块终点经纬度
        print("steplat" + steplat + "steplng" + steplng);//steplat0.5729578steplng3.71444



		///////////////////////////////
		arrObj[0].AddComponent<drawJterrain>().initTrr(lat + steplat, lng-steplng, lat , lng , "Trr00");
        arrTrr[0] = GameObject.Find("Trr00");
        arrObj[1].AddComponent<drawJterrain>().initTrr(lat + steplat, lng            , lat , lng + steplng, "Trr01");
        arrTrr[1] = GameObject.Find("Trr01");
        arrObj[2].AddComponent<drawJterrain>().initTrr(steplat + lat, lng+steplng, lat, lng + 2*steplng, "Trr02");
        arrTrr[2] = GameObject.Find("Trr02");

		arrObj[3].AddComponent<drawJterrain>().initTrr(lat, lng-steplng, lat-steplat , lng ,  "Trr10");
        arrTrr[3] = GameObject.Find("Trr10");
		arrObj[4].AddComponent<drawJterrain>().initTrr(lat, lng            , lat-steplat , lng +steplng,  "Trr11");
        arrTrr[4] = GameObject.Find("Trr11");
		arrObj[5].AddComponent<drawJterrain>().initTrr(lat, lng+steplng, lat-steplat , lng +2*steplng,  "Trr12");
        arrTrr[5] = GameObject.Find("Trr12");


		arrObj[6].AddComponent<drawJterrain>().initTrr(lat-steplat, lng-steplng, lat-2*steplat , lng , "Trr20");
        arrTrr[6] = GameObject.Find("Trr20");
		arrObj[7].AddComponent<drawJterrain>().initTrr(lat-steplat, lng            , lat-2*steplat , lng+steplng , "Trr21");
        arrTrr[7] = GameObject.Find("Trr21");
		arrObj[8].AddComponent<drawJterrain>().initTrr(lat-steplat, lng+steplng, lat-2*steplat , lng+2*steplng ,  "Trr22");
        arrTrr[8] = GameObject.Find("Trr22");


        arrTrr[0].transform.Translate(new Vector3(-100, -50, -100));
        arrTrr[1].transform.Translate(new Vector3(0     , -50, -100));
        arrTrr[2].transform.Translate(new Vector3(100 , -50, -100));
        arrTrr[3].transform.Translate(new Vector3(-100, -50, 0));
        arrTrr[4].transform.Translate(new Vector3(0     , -50, 0));
        arrTrr[5].transform.Translate(new Vector3(100 , -50, 0));
        arrTrr[6].transform.Translate(new Vector3(-100, -50, 100));
        arrTrr[7].transform.Translate(new Vector3(     0, -50 ,100));
        arrTrr[8].transform.Translate(new Vector3(100 , -50, 100));
		////////////////////////////////////////


    }

}