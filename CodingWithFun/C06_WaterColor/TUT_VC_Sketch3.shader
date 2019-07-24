// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-S-3" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		_MainTexB ("Sketch Texture", 2D) = "black" {}

		[Header(Shadow)]
		_ShadowThreshold("Threshold",Range(0, 1))=0
		_SoftEdge ("Soft Edge length", Range(0, 1)) = 1
		_ShadowStrength("Strength",Range(0, 1))=0
		[KeywordEnum(multiply, lighten)] _TYPE ("Type", Float) = 0

		_Darkcolor("Dark Color",COLOR)=(0, 0, 0, 1)

	}
	SubShader {
		Tags {"Queue"="Geometry" }
		CGINCLUDE
		#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MainTexB;

			half4 _MainTex_ST;
			half4 _MainTexB_ST;

			fixed4 _Darkcolor;

			fixed _ShadowThreshold;
			fixed _ShadowStrength;
			fixed _SoftEdge;

			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uvTB:TEXCOORD0;
				half2 uvTA[1] : TEXCOORD2;
			};

			v2f vert(appdata_img v) {
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uvTB=(v.texcoord-_MainTexB_ST.zw)*_MainTexB_ST.xy ;

				half2 uvTA = (v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
				o.uvTA[0] = uvTA;// + _MainTex_TexelSize.xy * half2(0, 0)  * size;


				return o;
			}

			fixed luminance(fixed4 color) {
				//GrayScale (0.222, 0.707, 0.071));
				return  0.2125 * color.r + 0.7154 * color.g + 0.0721 * color.b;
			}

	



			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[0]);
				fixed4 texBack=tex2D(_MainTexB, i.uvTB);

				fixed sh=luminance(texMain);
				fixed4 xx =texMain-_Darkcolor;
				//sh=abs(xx.x)+abs(xx.y)+abs(xx.z);
				sh=dot(xx,xx);
			
				
				//fixed soft=_SoftEdge>0? clamp(_ShadowThreshold-sh,0,_SoftEdge)/_SoftEdge : 1 ;
				fixed soft=saturate((_ShadowThreshold-sh)/_SoftEdge) ;
				sh=step(sh,_ShadowThreshold)*soft;
				fixed4 sum=texMain;
			
				#if _TYPE_MULTIPLY
					//Multiply
					fixed4 Csketch=texMain*lerp(1,texBack,_ShadowStrength);
					sum=lerp(texMain,Csketch,sh);
				#endif

				#if _TYPE_LIGHTEN
					//Lighten
					fixed4 Csketch=max( texMain,lerp(0,texBack,_ShadowStrength));
					sum=lerp(texMain,Csketch,sh);
				#endif
				return sum;

			}

	ENDCG


		Pass {

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0 
			#pragma multi_compile _TYPE_LIGHTEN _TYPE_MULTIPLY


			ENDCG
		}

	}
	FallBack "Diffuse"
}