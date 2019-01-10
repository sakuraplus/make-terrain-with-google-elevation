// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/PokemonLGPE swip" {
	Properties {
		_MainTex ("Texture A", 2D) = "white" {}
		_MainTexB ("Texture B", 2D) = "black" {}
		_Width("Width", Range(0,1) )=0.1
		[Space(10)]
		_Progress("Progress", Range(0,1))=0
	}


	SubShader {
		Tags {"Queue"="Transparent" }
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;
		sampler2D _MainTexB;

		uniform half4 _MainTex_ST;
		uniform half4 _MainTexB_ST;
		fixed _Progress;
		float _Width;
		half _SpeedCenter;

		fixed _Ratio;

		struct v2f {
			float4 pos : SV_POSITION;
			half2 uvTA: TEXCOORD0;
			half2 uvTB: TEXCOORD1;
			half2 uvORI: TEXCOORD2;//original
		};

		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			o.uvTA=(v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
			o.uvTB=(v.texcoord-_MainTexB_ST.zw)*_MainTexB_ST.xy ;
			o.uvORI=v.texcoord;

			return o;
		}



		fixed4 frag(v2f i) : SV_Target {
			fixed4 texA;
			fixed4 texB ;
			fixed4 color=fixed4(0,0,0,1);
		
			texA = tex2D(_MainTex, i.uvTA).rgba ;
			texB = tex2D(_MainTexB, i.uvTB).rgba ;			
				
				fixed GridY=0.5*_Width + floor(abs( i.uvORI.y-0.5)/_Width)*_Width;
				fixed EdgeX=0.5 + GridY/tan(_Progress*3.1415);
				if(i.uvORI.y>0.5){				
					i.uvORI.x=1-i.uvORI.x;
				}
			
//				EdgeX= floor(EdgeX/_Width)*_Width;				
//				if(i.uvORI.x<EdgeX){
//					color=texA;
//				}else{
//					color=texB;
//				}					
				
				fixed UVGridX=_Width+ floor(i.uvORI.x/_Width)*_Width;		
				fixed blend=(UVGridX-EdgeX)/_Width;
				blend=saturate(blend);	
				color=lerp(texA,texB,blend);	
	
			
			return color;
		}

		ENDCG


		Pass {

			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			ENDCG
		}


	}
	FallBack "Diffuse"
}