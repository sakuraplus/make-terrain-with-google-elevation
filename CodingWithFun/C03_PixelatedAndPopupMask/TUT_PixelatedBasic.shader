// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/PixelatedBasic01" {
	Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		_Progress ("Progress", Range(0,1)) = 1.0
  	
  	_SegmentX("Segment x", Float )=5
		_SegmentY("Segment y", Float )=5
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
   	uniform sampler2D _MainTex;
   	uniform float4 _MainTex_ST;
   	uniform float4 _MainTex_TexelSize;

		float _SegmentX;
		float _SegmentY;		
		float _Progress;			
			
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvORI: TEXCOORD0;
		};
		
		
		v2f vert(appdata_img v) {
			v2f o;

			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.uvORI=(v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;

			return o;
		}
		
		

		
		fixed4 frag(v2f i) : SV_Target {					
					float2 _Diameter=fixed2(1/ceil(_SegmentX),1/ceil(_SegmentY));
					float2 center = float2(_Diameter.x*floor(i.uvORI.x/_Diameter.x), _Diameter.y*floor(i.uvORI.y/_Diameter.y));
					center+=_Diameter/2;

//					return tex2D(_MainTex, center);

					float4 TexMain = tex2D(_MainTex, i.uvORI);
					
					float4 TexPixel = tex2D(_MainTex, center)*0.4;					
					TexPixel += tex2D(_MainTex, center+fixed2( _Diameter.x/4,0))*0.15;
					TexPixel += tex2D(_MainTex, center-fixed2( _Diameter.x/4,0))*0.15;
					TexPixel += tex2D(_MainTex, center+fixed2(0, _Diameter.y/4))*0.15;
					TexPixel += tex2D(_MainTex, center-fixed2(0, _Diameter.y/4))*0.15;		
							
					return lerp(TexMain,TexPixel,_Progress);
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


