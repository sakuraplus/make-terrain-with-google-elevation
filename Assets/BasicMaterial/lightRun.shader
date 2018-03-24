// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "test/lightRun" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MaskTex ("Mask Texture", 2D) = "white" {}
		_FirstColor("First Color", Color) = (1, 0, 0, 1)
		_EndColor("End Color", Color) = (1, 0, 0, 1)
		_ColorSpeed("color speed", Float) = 1.0
		_ColorNum("color num", Float) = 1.0
		
		
		_MainAddup ("Addup", Range(0,1)) = 1.0
		_IconTex ("Icon", 2D) = "white" {}
		_OffsetSize ("Offset", Float) = 1.0
		_ShadowNum ("Shadow Length", Range(0,15)) = 1.0
		_ShadowRange ("Shadow Alpha", Range(0,1)) = 0.5
		_rot ("Rotation", Range(0,360)) = 1.0


	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		sampler2D _MaskTex;  
		uniform half4 _MaskTex_ST;
		fixed4 _FirstColor;
		fixed4 _EndColor;
		float _ColorNum;
		float _ColorSpeed;
		
		
		float _MainAddup;
		uniform half4 _MainTex_ST;
		uniform half4 _IconTex_ST;
		sampler2D _IconTex;  
		half4 _IconTex_TexelSize;
		float _OffsetSize;
		float _ShadowNum;
		float _ShadowRange;
		fixed _rot; 

		struct v2f {
			float4 pos : SV_POSITION;
			half uv:TEXCOORD0;
			//half2 uv[15]: TEXCOORD0;
		};
		
		
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);

			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);			
			half2 uuv = v.texcoord;
			o.uv=uuv ;//+ _IconTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv-=_MainTex_ST.zw;
			o.uv*=_MainTex_ST.xy;
			return o;
		}
		
		

		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 sum = tex2D(_MainTex, i.uv).rgba;
			sum.rgb*=  _MainAddup;
			
//			fixed4 mask=tex2D(_MainTex, i.uv*_MaskTex_ST-_MaskTex_ST.zw).rgba;
//			if(mask.r>0.5){
//			sum.xyz+=_FirstColor;
//			}
			
			

		//	fixed4 icon = tex2D(_IconTex, i.uv[0]*_IconTex_ST.xy-_IconTex_ST.zw).rgba ;


		//	sum.xyz = lerp (sum.xyz, icon.xyz, icon.www);
			return sum;
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

