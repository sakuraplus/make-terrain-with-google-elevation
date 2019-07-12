// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-M-1" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		_MainTexM ("Texture Mask", 2D) = "black" {}

		[Header(Outline)]
		_OutlineThreshold ("Threshold", Range(0, 1)) = 0.1
		_OutlineSize ("Size ", Range(0, 1000)) = 0
		_Outlinecolor("Color",COLOR)=(0, 0, 0, 1)

	}
	SubShader {
		Tags {"Queue"="Geometry" }
		CGINCLUDE
		#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MainTexM;
			half4 _MainTex_ST;
			half4 _MainTexM_ST;

			uniform half4 _MainTex_TexelSize;
			fixed _OutlineThreshold;
			fixed _OutlineSize;
			fixed4 _Outlinecolor;

			sampler2D _CameraDepthNormalsTexture;


			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uvTM:TEXCOORD1;
				half2 uvTA[5] : TEXCOORD2;
			};

			v2f vert(appdata_img v) {
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uvTM=(v.texcoord-_MainTexM_ST.zw)*_MainTexM_ST.xy ;

				half2 uvTA = (v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
				fixed2 size=fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * _OutlineSize;
				o.uvTA[0] = uvTA + _MainTex_TexelSize.xy * half2(0, 0)  * size;
				o.uvTA[1] = uvTA + _MainTex_TexelSize.xy * half2(-1, -1)* size;
				o.uvTA[2] = uvTA + _MainTex_TexelSize.xy * half2(1, -1) * size;
				o.uvTA[3] = uvTA + _MainTex_TexelSize.xy * half2(-1, 1) * size;
				o.uvTA[4] = uvTA + _MainTex_TexelSize.xy * half2(1, 1)  * size;


				return o;
			}


//////////////////////////////			
			half RobertDepthN(v2f i) {
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0){
						i.uvTA[1].y=1-i.uvTA[1].y;
						i.uvTA[2].y=1-i.uvTA[2].y;
						i.uvTA[3].y=1-i.uvTA[3].y;
						i.uvTA[4].y=1-i.uvTA[4].y;
					}
				#endif		

				float4 LD = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[1].xy)));
				float4 RD = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[2].xy)));
				float4 LU = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[3].xy)));
				float4 RU = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[4].xy)));			
				
				half2 edgenormal2 =  abs(LD.rg-RU.rg) + abs(LU.rg-RD.rg);			
				half edgenormal=edgenormal2.r+edgenormal2.g;
				edgenormal=saturate((1-edgenormal)/(_OutlineThreshold));
					
				half edgedepth =  abs(DecodeFloatRG (LD.ba)-DecodeFloatRG (RU.ba)) + abs(DecodeFloatRG (LU.ba)-DecodeFloatRG (RD.ba));			
				edgedepth=saturate((1-edgedepth)/(_OutlineThreshold));
				return edgedepth*edgenormal;
					
			}


			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[0]);
				fixed4 texMask=tex2D(_MainTexM, i.uvTM);
				half edge =0;
				
				fixed4 onmask = lerp(texMain, texMask, texMask.a);
				edge = RobertDepthN(i);

				fixed ff=(1-edge)*(texMask.a)*_Outlinecolor.a;
				fixed4 sum=lerp(onmask,_Outlinecolor,ff);

				sum.a=texMain.a;
				return sum;
				

			}

	ENDCG


		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0 

			ENDCG
		}

	}
	FallBack "Diffuse"
}