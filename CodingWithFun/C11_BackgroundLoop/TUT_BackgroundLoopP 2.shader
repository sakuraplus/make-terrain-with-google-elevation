// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/BackgroundLoop_P2" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		
		_SpeedX("speedX",  Range(0,5)) = 1.0		
		_SpeedY("speedY",  Range(0,5)) = 1.0	
		[Space(10)]
		_Progress("Progress", Range(0,1))=0
	}
	SubShader {
		Tags {"Queue"="Transparent" }
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		uniform half4 _MainTex_ST;

		float _Progress;
		float _SpeedX,_SpeedY;
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvTA: TEXCOORD0;

		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uvTA=(v.texcoord)*_MainTex_ST.xy+fixed2(_SpeedX,_SpeedY)*_Progress -_MainTex_ST.zw ;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target {
			return tex2D(_MainTex, i.uvTA).rgba ;
		}
		    
		ENDCG


		Pass {

			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag
			ENDCG  
		}


	} 
	FallBack "Diffuse"
}