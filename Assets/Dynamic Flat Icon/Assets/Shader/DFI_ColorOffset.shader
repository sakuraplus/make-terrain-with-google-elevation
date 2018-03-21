// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/ColorOffset" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainAddup ("Addup", Range(0,1)) = 1.0
		_IconTex ("Icon", 2D) = "white" {}
		_RSize ("R Offset", Float) = 1.0
		_GSize ("G Offset", Float) = 1.0
		_BSize ("B Offset", Float) = 1.0
		_Rrot ("R Rotation", Range(0,360)) = 1.0
		_Grot ("G Rotation", Range(0,360)) = 1.0
		_Brot ("B Rotation", Range(0,360)) = 1.0
		[Toggle(_hideIcon)] _hideIcon("Hide Icon", float) = 1
		_RGBalpha ("Alpha", Range(0,2)) = 1.0

	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		float _MainAddup;
		sampler2D _IconTex;  
		half4 _IconTex_TexelSize;
		float _RSize;
		float _GSize;
		float _BSize;
		float _RGBalpha;
		
		//
		fixed _Rrot;
		fixed _Grot; 
		fixed _Brot;  
		uniform half4 _MainTex_ST;
		uniform half4 _IconTex_ST;
		//
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv[5]: TEXCOORD0;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);			
			half2 uv = v.texcoord;

			
			_Rrot=fmod(_Rrot,360);
			_Grot=fmod(_Grot,360);
			_Brot=fmod(_Brot,360);
			_Rrot=radians(_Rrot);
			_Grot=radians(_Grot);
			_Brot=radians(_Brot);
			half2 dirR=	(half2(sin(_Rrot),cos(_Rrot)));
			half2 dirG=	(half2(sin(_Grot),cos(_Grot)));
			half2 dirB=	(half2(sin(_Brot),cos(_Brot)));

			o.uv[0]=uv ;//+ _IconTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv[0]-=_MainTex_ST.zw;
			o.uv[0]*=_MainTex_ST.xy;	
			o.uv[1]=uv ;//+ _IconTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv[1]-=_IconTex_ST.zw;
			o.uv[1]*=_IconTex_ST.xy;		 
			o.uv[2]=uv + _IconTex_TexelSize.xy * half2(dirR.x, dirR.y)*_RSize*5;
			o.uv[2]-=_IconTex_ST.zw;
			o.uv[2]*=_IconTex_ST.xy;
			o.uv[3]=uv + _IconTex_TexelSize.xy * half2(dirG.x, dirG.y)*_GSize*5;
			o.uv[3]-=_IconTex_ST.zw;
			o.uv[3]*=_IconTex_ST.xy;		 
			o.uv[4]=uv + _IconTex_TexelSize.xy * half2(dirB.x, dirB.y)*_BSize*5;
			o.uv[4]-=_IconTex_ST.zw;
			o.uv[4]*=_IconTex_ST.xy;		 
			return o;
		}
		
	
		
		fixed4 frag(v2f i) : SV_Target {
			
			fixed4 sum = tex2D(_MainTex, i.uv[0]).rgba ;
			sum.rgb*=_MainAddup;

			
			sum.r+=tex2D(_IconTex, i.uv[2]).r*tex2D(_IconTex, i.uv[2]).a*_RGBalpha;
			sum.g+=tex2D(_IconTex, i.uv[3]).g*tex2D(_IconTex, i.uv[3]).a*_RGBalpha;
			sum.b+=tex2D(_IconTex, i.uv[4]).b*tex2D(_IconTex, i.uv[4]).a*_RGBalpha;
			#ifdef _hideIcon
				return sum;
     	#endif
				
				fixed4 icon = tex2D(_IconTex, i.uv[1]).rgba;
				sum.rgb = lerp (sum.rgb, icon.xyz, icon.www);
				return sum;
		}
		    
		ENDCG
	
		
		Pass {
			
			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag
			#pragma shader_feature _hideIcon
			  
			ENDCG  
		}
		
		
	} 
	FallBack "Diffuse"
}
