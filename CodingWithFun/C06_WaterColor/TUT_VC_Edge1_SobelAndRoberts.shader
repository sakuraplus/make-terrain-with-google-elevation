// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-E-1" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}

		[Header(Outline)]
		_OutlineThreshold ("Threshold", Range(0, 4)) = 0.1
		_OutlineSize ("Size ", Range(0, 1)) = 0
		_Outlinecolor("Color",COLOR)=(0, 0, 0, 1)
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


			struct v2f {
				float4 pos : SV_POSITION;
				half2 uvTA[9] : TEXCOORD2;
			};

			v2f vert(appdata_img v) {
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				half2 uvTA = (v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;

				o.uvTA[0] = uvTA + _MainTex_TexelSize.xy * half2(-1, -1)* _OutlineSize;
				o.uvTA[1] = uvTA + _MainTex_TexelSize.xy * half2(0, -1) * _OutlineSize;
				o.uvTA[2] = uvTA + _MainTex_TexelSize.xy * half2(1, -1) * _OutlineSize;
				o.uvTA[3] = uvTA + _MainTex_TexelSize.xy * half2(-1, 0) * _OutlineSize;
				o.uvTA[4] = uvTA + _MainTex_TexelSize.xy * half2(0, 0)  * _OutlineSize;
				o.uvTA[5] = uvTA + _MainTex_TexelSize.xy * half2(1, 0)  * _OutlineSize;
				o.uvTA[6] = uvTA + _MainTex_TexelSize.xy * half2(-1, 1) * _OutlineSize;
				o.uvTA[7] = uvTA + _MainTex_TexelSize.xy * half2(0, 1)  * _OutlineSize;
				o.uvTA[8] = uvTA + _MainTex_TexelSize.xy * half2(1, 1)  * _OutlineSize;


				return o;
			}

			fixed luminance(fixed4 color) {
				//GrayScale (0.222, 0.707, 0.071));
				return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
			}

			half Sobel(v2f i) {
				const half Gx[9] = {-1,  0,  1,
														-2,  0,  2,
														-1,  0,  1};
				const half Gy[9] = {-1, -2, -1,
														 0,  0,  0,
														 1,  2,  1};

				half texColor;
				half edgeX = 0;
				half edgeY = 0;
				for (int it = 0; it < 9; it++) {
					texColor = luminance(tex2D(_MainTex, i.uvTA[it]));
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



			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[4]);

				half edge =0;

					#if _OPERATOR_SOBEL						
						edge = Sobel(i);
					#endif
					#if _OPERATOR_ROBERT
						edge = Robert(i);
					#endif	

				fixed4 sum=lerp(_Outlinecolor,texMain,edge);
				
				sum.a=texMain.a;
				return sum;

			}

	ENDCG


		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0 
			#pragma multi_compile _OPERATOR_SOBEL _OPERATOR_ROBERT

			ENDCG
		}

	}
	FallBack "Diffuse"
}