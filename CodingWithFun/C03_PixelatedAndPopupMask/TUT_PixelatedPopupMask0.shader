// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/PixelatedPopupMask00" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainTexM ("Texture Mask", 2D) = "black" {}				
		_Scale ("Scale", Float) = 1.0
		_SegmentX("Segment x", Float )=5
		_SegmentY("Segment y", Float )=5
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		sampler2D _MainTexM; 
		uniform half4 _MainTexM_ST; 
		uniform half4 _MainTex_ST;

		float _SegmentX;
		float _SegmentY;		
		float _Scale;			
			
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvORI: TEXCOORD1;//original
		};
		
		v2f vert(appdata_img v) {
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uvORI=(v.texcoord-_MainTexM_ST.zw)*_MainTexM_ST.xy ;
			return o;
		}
	
		fixed4 frag(v2f i) : SV_Target {					
					float2 _Diameter=fixed2(1/ceil(_SegmentX),1/ceil(_SegmentY));
					float2 center = float2(_Diameter.x*floor(i.uvORI.x/_Diameter.x), _Diameter.y*floor(i.uvORI.y/_Diameter.y));
					center+=_Diameter/2;

					float2 _rd=_Scale*_Diameter/2;
					fixed4 TexMain = tex2D(_MainTex, i.uvORI);
					
					fixed2 uvMask=fixed2((i.uvORI.x-center.x),(i.uvORI.y-center.y));
					uvMask=(uvMask+_rd)/(2*_rd);	
					uvMask=saturate(uvMask);
					
					fixed4 TexMask= tex2D(_MainTexM,uvMask) ;

					return  lerp (TexMain, TexMask,TexMask.a);
			}
   ENDCG
    Pass {
      CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
      ENDCG
    }
  }
  FallBack "Diffuse"
}
