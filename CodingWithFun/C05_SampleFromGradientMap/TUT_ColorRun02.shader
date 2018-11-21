// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/ColorRun02" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}

		_ColorNum("color num", Float) = 2.0		
		_ColorSpeed("color speed", Float) = 1.0

		_MaskTex ("Mask", 2D) = "white" {}
		_GradientMap("Gradient Map", 2D) = "white" {}
	}
	SubShader {
		Tags {"Queue"="Transparent" }

		CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		sampler2D _MaskTex;  
		sampler2D _GradientMap;
		fixed4 _ColorStart;
		fixed4 _ColorEnd;
		float _ColorNum;
		float _ColorSpeed;

		uniform half4 _MainTex_ST;
		uniform half4 _MaskTex_ST;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvMain: TEXCOORD0;
			half2 uvMask: TEXCOORD1;
			half uvColor: TEXCOORD2;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			o.uvMain=(v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
			o.uvMask=(v.texcoord-_MaskTex_ST.zw)*_MaskTex_ST.xy ;

 			int indcolor=floor(_ColorNum*(sin(_Time.y * _ColorSpeed)+1 )/2);
			o.uvColor=saturate( indcolor/(_ColorNum-1));
			
			return o;
		}



		fixed4 frag(v2f i) : SV_Target {

			fixed4 sum = tex2D(_MainTex, i.uvMain).rgba ;			
			fixed4 mask = tex2D(_MaskTex, i.uvMask).rgba;
			fixed4 lightcolor	=tex2D(_GradientMap, fixed2(i.uvColor,0.5));
			//fixed4 lightcolor	=lerp (_ColorStart, _ColorEnd,i.uvColor.x );
			sum.rgb += (lightcolor.a*lightcolor.rgb*mask.a );			
			return sum;
		}
		    
		ENDCG


		Pass {
			Tags { "LightMode"="ForwardBase" }

			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag
			  
			ENDCG  
		}


	} 
	FallBack "Diffuse"
}