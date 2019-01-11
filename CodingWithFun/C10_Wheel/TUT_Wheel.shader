// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP , * )' with 'UnityObjectToClipPos( * )'
//SlideshowEffect2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/Wheel" {
		Properties {
		_Progress("Progress" , Range(0 , 1))=0
				
		_MainTex  ("Texture A" , 2D) = "white" {}
		_MainTexB ("Texture B" , 2D) = "black" {}

		_Num("num" , Float )=4

		_Edge("Soft Edge" , Range(0 , 0.5)) = 0.1
		_OffsetR ("rotation start" , Range(0 , 7)) = 1.0
	}
	SubShader {
  	Tags {"Queue"="Transparent" }
		//Tags { "RenderType"="Opaque" }
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex; 
		sampler2D _MainTexB; 		
		uniform half4 _MainTex_ST;
		uniform half4 _MainTexB_ST;	
		
		half _OffsetR;
	
		half _Progress;
		half _Num;
		half _Edge;
	
		struct v2f {
			float4 pos : SV_POSITION;
			half2  uvTA: TEXCOORD0;
			half2  uvTB: TEXCOORD1;
			half2  uvORI: TEXCOORD2; // original
		};
		 
		v2f vert(appdata_img v) {
			v2f o;
			 // o.pos = UnityObjectToClipPos(v.vertex); // mul(UNITY_MATRIX_MVP , v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP , v.vertex);			

			o.uvTA  = (v.texcoord - _MainTex_ST.zw) * _MainTex_ST.xy ;
			o.uvTB  = (v.texcoord - _MainTexB_ST.zw) * _MainTexB_ST.xy ;
			o.uvORI = v.texcoord-0.5;
			return o;
		}
		
	
		
		fixed4 frag(v2f i) : SV_Target {					
			fixed4 texA;
			fixed4 texB ;			
			fixed4 sum=fixed4(1,1,1,1);

			half rotPre=6.28 / floor( _Num);
			half rotProgress=_Progress * (rotPre+_Edge)-_Edge;

			texB= tex2D(_MainTex , i.uvTA).rgba;
			texA= tex2D(_MainTexB , i.uvTB).rgba;					
				
			half rot=3.14+atan2(i.uvORI.x,i.uvORI.y);
			half fm=fmod(rot+_OffsetR , rotPre);


			sum=lerp(texB , texA ,saturate( (rotProgress+_Edge - fm) / _Edge));			

//		  if( fm>=rotProgress &&  fm < rotProgress+_Edge){
//			  sum=lerp(texB , texA , (rotProgress+_Edge - fm) / _Edge);				
//			}else 
//			if( fm<=rotProgress+_Edge){
//				sum = texA;
//			}	else{
//				sum = texB;
//			}					
			
			
			return sum;					
		}
		 
		ENDCG
	
		
		Pass {
			
			// ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			 
			#pragma vertex vert 
			#pragma fragment frag

			ENDCG 
		}
		
		
	} 
	FallBack "Diffuse"
}