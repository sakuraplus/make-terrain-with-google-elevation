// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/ColorRun00" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}

		_FirstColor("First Color", Color) = (0, 0, 0, 1)
		_EndColor("End Color", Color) = (1, 0, 0, 1)
		
		_ColorNum("color num", Float) = 2.0
		_ColorSpeed("color speed", Float) = 1.0

		_MaskTex ("Mask", 2D) = "white" {}
		
	}
	SubShader {
		Tags {"Queue"="Transparent" }

		CGINCLUDE
		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		sampler2D _MaskTex;  

		fixed4 _FirstColor;
		fixed4 _EndColor;
		float _ColorNum;
		float _ColorSpeed;

		uniform half4 _MainTex_ST;
		uniform half4 _MaskTex_ST;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv: TEXCOORD0;
			half2 uvMask: TEXCOORD1;
			half2 uvColor: TEXCOORD2;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			o.uv=(v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
			o.uvMask=(v.texcoord-_MaskTex_ST.zw)*_MaskTex_ST.xy ;

 			int indcolor=floor(_ColorNum*(sin(_Time.y * _ColorSpeed)+1 )/2);

			o.uvColor.x=clamp( indcolor/(_ColorNum-1),0,1);
			o.uvColor.y=1;
			return o;
		}



		fixed4 frag(v2f i) : SV_Target {

			fixed4 sum = tex2D(_MainTex, i.uv).rgba ;

			fixed4 lightcolor	=lerp (_FirstColor, _EndColor,i.uvColor.x );
			fixed4 mask = tex2D(_MaskTex, i.uvMask).rgba;

			fixed f= mask.a;
			sum.rgb += (lightcolor.a*lightcolor.xyz*f );			
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