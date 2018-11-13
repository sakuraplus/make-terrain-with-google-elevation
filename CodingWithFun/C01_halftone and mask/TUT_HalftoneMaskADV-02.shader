// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/HalftoneMask_ADV(B)02" {
	Properties {
		_MainTex ("Texture A", 2D) = "black" {}
		_MainTexB ("Texture B", 2D) = "black" {}
		[Space(10)]
		_Position("Halftone Position", Float)=1
		_Diameter("Diameter", Range(0,1) )=0.25	
		_Num("Length",  Range(1,16)) = 3.0

	}


	SubShader {
		Tags {"Queue"="Transparent" }
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		sampler2D _MainTexB;  

		uniform half4 _MainTex_ST;
		uniform half4 _MainTexB_ST;

		float _Diameter;
		float _Position;
		float _Num;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvTA: TEXCOORD0;
			half2 uvTB: TEXCOORD1;
			half2 uvORI: TEXCOORD2;//original
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uvTA=(v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
			o.uvTB=(v.texcoord-_MainTexB_ST.zw)*_MainTexB_ST.xy ;
			o.uvORI.xy=v.texcoord;

			return o;
		}



		fixed4 frag(v2f i) : SV_Target {
			float _rd;
			fixed2 posCenter;
			fixed diameterW,diameterH;
			diameterW=_Diameter;	
			diameterH=_Diameter;		
			fixed indexOfGrid=floor((i.uvORI.x-_Position)/diameterW);//num of grids between uv and PosW
			
			if(indexOfGrid>=1){
						return tex2D(_MainTexB, i.uvTB).rgba ;//return texture A when uv is in front (larger) of PosW
			}
			
			posCenter.x=_Position+(indexOfGrid + 0.5)*diameterW;		
			posCenter.y=(floor(i.uvORI.y/diameterH) + 0.5)*diameterH;		

			_rd=0.5*_Diameter* abs(indexOfGrid)/_Num;//radius of the current grid 			
			float _rdNext=_rd+0.5*_Diameter/_Num;
			fixed2 posCenterNext=fixed2(0,0);//center of up-next grid 
			posCenterNext=posCenter-fixed2(diameterW,0);	
			
			//fixed inCircle=step(abs(i.uvORI.x-posCenter.x),_rd)*step(abs(i.uvORI.y-posCenter.y),_rd);	 //Square
			fixed inCircle=step(distance(i.uvORI,posCenter),_rd)+step(distance(i.uvORI,posCenterNext),_rdNext);	//Dot
			inCircle=saturate(inCircle);

			fixed4 texA=tex2D(_MainTex, i.uvTA).rgba;
			fixed4 texB=tex2D(_MainTexB, i.uvTB).rgba;
			fixed4 sum= lerp(texB,texA,inCircle);
			return sum;		
		}
		    
		ENDCG


		Pass {

			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag		
			#pragma target 3.0
			  
			ENDCG  
		}

	} 
	FallBack "Diffuse"
}