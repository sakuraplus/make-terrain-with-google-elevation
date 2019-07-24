// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-E-2" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}

		[Header(Outline)]
		_OutlineThreshold ("Threshold", Range(0, 1)) = 0.1
		_OutlineSize ("Size ", Range(0, 1000)) = 0
		_Outlinecolor("Color",COLOR)=(0, 0, 0, 1)
		
		[KeywordEnum(depth, luminance)] _TYPE ("Type", Float) = 0
		[KeywordEnum(Sobel, Robert)] _OPERATOR ("operator", Float) = 0
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
				o.uvTA[4] = uvTA + _MainTex_TexelSize.xy * half2(0, 0)  * size;
				o.uvTA[5] = uvTA + _MainTex_TexelSize.xy * half2(1, 0)  * size;
				o.uvTA[6] = uvTA + _MainTex_TexelSize.xy * half2(-1, 1) * size;
				o.uvTA[7] = uvTA + _MainTex_TexelSize.xy * half2(0, 1)  * size;
				o.uvTA[8] = uvTA + _MainTex_TexelSize.xy * half2(1, 1)  * size;


				return o;
			}

			fixed luminance(fixed4 color) {
				//GrayScale (0.222, 0.707, 0.071));
				//return color.b;
				return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
			}
		
			half Sobel(v2f i) {
				const half Gx[9] = {-1,  0,  1,	-2,  0,  2,	-1,  0,  1};
				const half Gy[9] = {-1, -2, -1,	 0,  0,  0,	 1,  2,  1};

				half texColor;
				half edgeX = 0;
				half edgeY = 0;
				for (int it = 0; it < 9; it++) {
					
				#if _TYPE_DEPTH
					#if UNITY_UV_STARTS_AT_TOP
						if (_MainTex_TexelSize.y < 0){
							i.uvTA[it].y=1-i.uvTA[it].y;						
						}
					#endif	
					texColor =Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvTA[it].xy)));
				#else
				//#if _TYPE_LUMINANCE		
					texColor = luminance(tex2D(_MainTex, i.uvTA[it]));
				#endif
					edgeX += texColor * Gx[it];
					edgeY += texColor * Gy[it];
				}

			
				half edge =abs(edgeX) + abs(edgeY);
				//edge=sqrt((edgeX*edgeX) + (edgeY*edgeY));
				edge=saturate((1-edge)/_OutlineThreshold);
				return edge;		
			
			}


			
			half Robert(v2f i) {
					
				half LD =luminance( tex2D(_MainTex, i.uvTA[0].xy));
				half RD =luminance( tex2D(_MainTex, i.uvTA[2].xy));
				half LU =luminance( tex2D(_MainTex, i.uvTA[6].xy));
				half RU =luminance( tex2D(_MainTex, i.uvTA[8].xy));
				
				//half edge = sqrt( (LD-RU) *(LD-RU) + (LU-RD)*(LU-RD));	
				//half edge =abs(LU-RD) * abs(LD-RU);
				half edge =  abs(LD-RU) + abs(LU-RD);
			
				edge=saturate((1-edge)/_OutlineThreshold);
				return edge;
			}			

			
			half RobertDepth(v2f i) {
				#if UNITY_UV_STARTS_AT_TOP
					if (_MainTex_TexelSize.y < 0){
						i.uvTA[0].y=1-i.uvTA[0].y;
						i.uvTA[2].y=1-i.uvTA[2].y;
						i.uvTA[6].y=1-i.uvTA[6].y;
						i.uvTA[8].y=1-i.uvTA[8].y;
					}
				#endif		

				float LD = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvTA[0].xy)));
				float RD = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvTA[2].xy)));
				float LU = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvTA[6].xy)));
				float RU = Linear01Depth(UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, i.uvTA[8].xy)));					
			
				half edge =  abs(LD-RU) + abs(LU-RD);			
				edge=saturate((1-edge)/(_OutlineThreshold));

				return edge;
			}
			





			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[4]);

				half edge =0;
				
				
				#if _TYPE_LUMINANCE						
					#if _OPERATOR_SOBEL
						edge = Sobel(i);
					#endif
					#if _OPERATOR_ROBERT
						edge = Robert(i);
					#endif					
				#endif
				
				#if _TYPE_DEPTH
				
				
				   float d = UNITY_SAMPLE_DEPTH(tex2D(_CameraDepthTexture, fixed2(i.uvTA[4].x,1-i.uvTA[4].y)));
           d = Linear01Depth(d);
           texMain= fixed4(d,d,d,1);           
           
					//texMain=tex2D(_CameraDepthTexture, fixed2(i.uvTA[4].x,1-i.uvTA[4].y));
					//texMain=fixed4( texMain.r,texMain.r,texMain.r,1);
			
					#if _OPERATOR_SOBEL						
						edge = Sobel(i);
					#endif
					#if _OPERATOR_ROBERT
						edge = RobertDepth(i);
					#endif										
				#endif
						
					
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

			#pragma multi_compile _TYPE_DEPTH   _TYPE_LUMINANCE
			#pragma multi_compile _OPERATOR_SOBEL _OPERATOR_ROBERT

			ENDCG
		}

	}
	FallBack "Diffuse"
}