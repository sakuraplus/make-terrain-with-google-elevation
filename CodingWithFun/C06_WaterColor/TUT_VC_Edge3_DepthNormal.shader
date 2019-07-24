// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-E-3" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}

		[Header(Outline)]
		_OutlineThreshold ("Threshold", Range(0, 1)) = 0.1
		_OutlineSize ("Size ", Range(0, 1000)) = 0
		_Outlinecolor("Color",COLOR)=(0, 0, 0, 1)
		
		[KeywordEnum(DepthNormal, test)] _TYPE ("Type", Float) = 0
	}
	SubShader {
		Tags {"Queue"="Geometry" }
		CGINCLUDE
		#include "UnityCG.cginc"

			sampler2D _MainTex;
			half4 _MainTex_ST;


			uniform half4 _MainTex_TexelSize;
			fixed _OutlineThreshold;
			fixed _OutlineSize;
			fixed4 _Outlinecolor;

			sampler2D _CameraDepthNormalsTexture;
			sampler2D_float _CameraDepthTexture;


			struct v2f {
				float4 pos : SV_POSITION;
				half2 uvTA[9] : TEXCOORD2;
			};

			v2f vert(appdata_img v) {
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				half2 uvTA = (v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
				fixed2 size=fixed2(_MainTex_TexelSize.x, _MainTex_TexelSize.y) * _OutlineSize;
				o.uvTA[0] = uvTA + _MainTex_TexelSize.xy * half2(-1, -1)* size;
				o.uvTA[1] = uvTA + _MainTex_TexelSize.xy * half2(0, -1) * size;
				o.uvTA[2] = uvTA + _MainTex_TexelSize.xy * half2(1, -1) * size;
				o.uvTA[3] = uvTA + _MainTex_TexelSize.xy * half2(-1, 0) * size;
				o.uvTA[4] = uvTA;// + _MainTex_TexelSize.xy * half2(0, 0)  * size;
				o.uvTA[5] = uvTA + _MainTex_TexelSize.xy * half2(1, 0)  * size;
				o.uvTA[6] = uvTA + _MainTex_TexelSize.xy * half2(-1, 1) * size;
				o.uvTA[7] = uvTA + _MainTex_TexelSize.xy * half2(0, 1)  * size;
				o.uvTA[8] = uvTA + _MainTex_TexelSize.xy * half2(1, 1)  * size;


				return o;
			}

		
			half RobertDepthN(v2f i) {
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0){
						i.uvTA[0].y=1-i.uvTA[0].y;
						i.uvTA[2].y=1-i.uvTA[2].y;
						i.uvTA[6].y=1-i.uvTA[6].y;
						i.uvTA[8].y=1-i.uvTA[8].y;
					}
				#endif		

				float4 LD = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[0].xy)));
				float4 RD = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[2].xy)));
				float4 LU = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[6].xy)));
				float4 RU = ((tex2D(_CameraDepthNormalsTexture, i.uvTA[8].xy)));			
				
				half2 edgenormal2 =  abs(LD.rg-RU.rg) + abs(LU.rg-RD.rg);			
				half edgenormal=edgenormal2.r+edgenormal2.g;
				edgenormal=saturate((1-edgenormal)/(_OutlineThreshold));
					
				half edgedepth =  abs(DecodeFloatRG (LD.ba)-DecodeFloatRG (RU.ba)) + abs(DecodeFloatRG (LU.ba)-DecodeFloatRG (RD.ba));			
				edgedepth=saturate((1-edgedepth)/(_OutlineThreshold));
				return edgedepth*edgenormal;
					
			}


			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[4]);

				half edge =0;
				

				
				
				
				#if _TYPE_DEPTHNORMAL
					texMain=tex2D(_CameraDepthNormalsTexture,fixed2(i.uvTA[4].x,1-i.uvTA[4].y));
					


				#endif			
				
				#if _TYPE_TEST
					texMain=tex2D(_CameraDepthNormalsTexture,fixed2(i.uvTA[4].x,1-i.uvTA[4].y));
					texMain=fixed4( texMain.r,texMain.g,1,1);
					
					//see how b,a channel works
					//texMain=fixed4( texMain.b,texMain.b,texMain.b,1);
					//texMain=fixed4( texMain.a,texMain.a,texMain.a,1);
					
					//see normal and depth with DecodeDepthNormal
					//float dep;
					//float3 nom;
					//DecodeDepthNormal (texMain, dep, nom);
					//texMain=fixed4( dep,dep,dep,1);
					//texMain=fixed4( nom,1);
					
					//how DecodeFloatRG work
					//Float fd=DecodeFloatRG(texMain.ba);
					//texMain=fixed4( fd,fd,fd,1);
	

				#endif		
				
				edge = RobertDepthN(i);
				
				fixed ff=(1-edge)*_Outlinecolor.a;
				fixed4 sum=lerp(texMain,_Outlinecolor,ff);

					
				return sum;

			}

	ENDCG


		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0 
			#pragma multi_compile  _TYPE_DEPTHNORMAL _TYPE_TEST 

			ENDCG
		}

	}
	FallBack "Diffuse"
}