// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/FlatShadow_decay" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainAddup ("Addup", Range(0,1)) = 1.0
		_IconTex ("Icon", 2D) = "white" {}
		_OffsetSize ("Offset", Float) = 1.0
		_ShadowNum ("Shadow Length", Range(0,15)) = 1.0
		_ShadowRange ("Shadow Alpha", Range(0,1)) = 0.5
		_rot ("Rotation", Range(0,360)) = 1.0
		_decay ("Decay", Range(0.6,1)) = 1.0
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		float _MainAddup;
		uniform half4 _MainTex_ST;
		uniform half4 _IconTex_ST;
		sampler2D _IconTex;  
		half4 _IconTex_TexelSize;
		float _OffsetSize;
		float _ShadowNum;
		float _ShadowRange;
		fixed _rot; 
		fixed _decay; 

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv[15]: TEXCOORD0;
		};
		
		
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			_rot=fmod(_rot,360);
			_rot=radians(_rot);
			half2 dir=	(-1*half2(sin(_rot),-1*cos(_rot)));//normalize
			half2 uv = v.texcoord;

			for (int it = 0; it < _ShadowNum; it++) {
				o.uv[it]=uv +  _IconTex_TexelSize*half2(it*dir.x, it*dir.y)*_OffsetSize;

			}		 
					 
			return o;
		}
		
		

		
		fixed4 frag(v2f i) : SV_Target {
			fixed4 sum = tex2D(_MainTex, i.uv[0]*_MainTex_ST-_MainTex_ST.zw).rgba;//* 0.5;
			sum.rgb*=_MainAddup;
			fixed iconshadow=0;
			for (int it =_ShadowNum-1; it >=0; it--) {
				if(it>14){
					it=14;
				}
				fixed shadowA = tex2D(_IconTex, i.uv[it]*_IconTex_ST-_IconTex_ST.zw).a ;
				if(iconshadow<shadowA){
					iconshadow=shadowA;
				}
				else{
					iconshadow*=_decay;
				}
			}
			
			if(iconshadow>0){

				float sr=lerp(1,1-_ShadowRange,iconshadow);
				sum.xyz *=sr;
			}
			fixed4 icon = tex2D(_IconTex, i.uv[0]*_IconTex_ST.xy-_IconTex_ST.zw).rgba ;


			sum.xyz = lerp (sum.xyz, icon.xyz, icon.www);
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
