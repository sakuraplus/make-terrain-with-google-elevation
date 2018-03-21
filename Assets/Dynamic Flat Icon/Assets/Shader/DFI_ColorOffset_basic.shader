// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/ColorOffset_basic" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainAddup ("Addup", Range(0,1)) = 1.0
		_IconTex ("Icon", 2D) = "white" {}
		_OffsetSize ("Offset", Float) = 1.0
		[Toggle(_hideIcon)] _hideIcon("Hide Icon", float) = 1
		_RGBalpha ("RGBalpha", Range(0,2)) = 1.0
		_rot ("Rotation", Range(0,360)) = 1.0

	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		sampler2D _IconTex;  
		half4 _IconTex_TexelSize;
		float _OffsetSize;
		float _RGBalpha;
		float _MainAddup;
		//
		fixed _rot; 
		uniform half4 _MainTex_ST;
		uniform half4 _IconTex_ST;
		//
		
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv[6]: TEXCOORD0;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);			
			half2 uv = v.texcoord;
			//half2 dir=	normalize(half2(_Dirx,_Diry));
			
			_rot=fmod(_rot,360);
			fixed _rotG=(_rot+120)%360;
			fixed _rotB=(_rot+240)%360;
			_rot=radians(_rot);
			half2 dir=	(half2(sin(_rot),cos(_rot)));//normalize
			_rotG=radians(_rotG);
			half2 dirG=	(half2(sin(_rotG),cos(_rotG)));//normalize
			_rotB=radians(_rotB);
			half2 dirB=	(half2(sin(_rotB),cos(_rotB)));//normalize

			o.uv[0]=uv ;//+ _IconTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv[0]-=_MainTex_ST.zw;
			o.uv[0]*=_MainTex_ST.xy;	
			o.uv[1]=uv ;//+ _IconTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv[1]-=_IconTex_ST.zw;
			o.uv[1]*=_IconTex_ST.xy;		 
			o.uv[2]=uv + _IconTex_TexelSize.xy * half2(dir.x, dir.y)*_OffsetSize;
			o.uv[2]-=_IconTex_ST.zw;
			o.uv[2]*=_IconTex_ST.xy;
			o.uv[3]=uv + _IconTex_TexelSize.xy * half2(dirG.x, dirG.y)*_OffsetSize;
			o.uv[3]-=_IconTex_ST.zw;
			o.uv[3]*=_IconTex_ST.xy;		 
			o.uv[4]=uv + _IconTex_TexelSize.xy * half2(dirB.x, dirB.y)*_OffsetSize;
			o.uv[4]-=_IconTex_ST.zw;
			o.uv[4]*=_IconTex_ST.xy;		 	
			o.uv[5]=uv+half2(dir.x, dir.y)*0.1; 
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
