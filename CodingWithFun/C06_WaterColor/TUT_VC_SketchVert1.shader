// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//SlideshowEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "tut/VC-SV-1" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		_MainTexB ("Sketch Texture", 2D) = "black" {}

		[Header(Shadow)]
		_ShadowThreshold("Threshold",Range(0, 1))=0
		_SoftEdge ("Soft Edge length", Range(0, 1)) = 1
		_ShadowStrength("Strength",Range(0, 1))=0
		[KeywordEnum(multiply, lighten)] _TYPE ("Type", Float) = 0
//		[Header(Outline)]
//		_OutlineThreshold ("Threshold", Range(0, 4)) = 0.1
//		_OutlineSize ("Size ", Range(0, 1)) = 0
//		_Darkcolor("Dark Color",COLOR)=(0, 0, 0, 1)

	}
	SubShader {
		Tags {"Queue"="Geometry" }
		CGINCLUDE
		#include "UnityCG.cginc"

			sampler2D _MainTex;
			sampler2D _MainTexB;

			half4 _MainTex_ST;
			half4 _MainTexB_ST;


			fixed _ShadowThreshold;
			fixed _ShadowStrength;
			fixed _SoftEdge;

			struct v2f {
				float4 pos : SV_POSITION;
				fixed2 uvTB:TEXCOORD0;
				half2 uvTA[1] : TEXCOORD2;
				fixed sketchWeights : TEXCOORD3;
			};

			v2f vert(appdata_full v) {
				v2f o;
				//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uvTB=(v.texcoord-_MainTexB_ST.zw)*_MainTexB_ST.xy ;
				half2 uvTA = (v.texcoord-_MainTex_ST.zw)*_MainTex_ST.xy ;
				o.uvTA[0] = uvTA;// + _MainTex_TexelSize.xy * half2(0, 0)  * size;

				
				
				fixed3 worldLightDir = normalize(WorldSpaceLightDir(v.vertex));
				fixed3 worldNormal = UnityObjectToWorldNormal(v.normal);
				fixed 	lightDifference =dot(worldLightDir, worldNormal); //from -1 to 1
				lightDifference=(lightDifference+1)/2; //from 0 to 1
				
				//fixed soft=_SoftEdge>0? clamp(_ShadowThreshold-lightDifference,0,_SoftEdge)/_SoftEdge : 1 ;
				fixed soft=saturate((_ShadowThreshold-lightDifference)/_SoftEdge) ;
				lightDifference=step(lightDifference,_ShadowThreshold)*soft;
				o.sketchWeights=saturate(lightDifference);
				

				return o;
			}



			fixed4 frag(v2f i) : SV_Target {
				fixed4 texMain=tex2D(_MainTex, i.uvTA[0]);
				fixed4 texBack=tex2D(_MainTexB, i.uvTB);

				fixed4 sum=texMain;
				
				#if _TYPE_LIGHTEN		
					fixed4 Csketch=max( texMain,lerp(0,texBack,_ShadowStrength));
					sum.rgb=lerp(texMain,Csketch,i.sketchWeights);		
				#endif			
			
				#if _TYPE_MULTIPLY		
					fixed4 Csketch= texMain*lerp(1,texBack,_ShadowStrength);
					sum.rgb=lerp(texMain,Csketch,i.sketchWeights);		
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