// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/BackgroundLoopT2" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}			
//		_uvx("uv offset X",  Float) = 0.0		
//		_uvy("uv offset Y",  Float) = 0.0	
	}
	SubShader {
		Tags {"Queue"="Transparent" }
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		uniform half4 _MainTex_ST;
		
		float _uvx,_uvy;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvTA: TEXCOORD0;

		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uvTA=(v.texcoord*_MainTex_ST.xy-_MainTex_ST.zw)+fixed2( _uvx,_uvy);
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
		  
			#pragma shader_feature _FlipY	
			ENDCG  
		}


	} 
	FallBack "Diffuse"
}