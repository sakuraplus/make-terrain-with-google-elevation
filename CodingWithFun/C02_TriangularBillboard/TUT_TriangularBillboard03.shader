// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/TriangularBillboard03" {
	Properties {
		_MainTex ("Texture A", 2D) = "white" {}
		_MainTexB ("Texture B", 2D) = "black" {}
		//[Toggle(_FlipY)] _FlipY ("Flip Y", Float) = 0
		[Space(10)]
		_Progress("Progress", Range(0,1))=0
		_Width("Width",Float )=0.5
		_ShadowStrength("Shadow",  Range(0,2)) = 1.0
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
		half _ShadowStrength;
		
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

			o.uvORI=v.texcoord;

			return o;
		}



		fixed4 frag(v2f i) : SV_Target {
			fixed4 texA;
			fixed4 texB ;
			fixed4 color;


			
			
			float	posmod= fmod(abs(i.uvORI.x),_Width);
		
			fixed2	uvA=fixed2(i.uvTA.x+(posmod-_Width)* _Progress/(1-_Progress), i.uvTA.y);
			fixed2	uvB=fixed2(i.uvTB.x+(1-_Progress)*posmod /_Progress, i.uvTB.y);
			texA = tex2D(_MainTex, uvA).rgba ;
			texB = tex2D(_MainTexB, uvB).rgba ;			
			
			fixed shadowOnTexA=_ShadowStrength*_Progress;
			shadowOnTexA *= (posmod-_Width*_Progress)/(_Width*(1-_Progress));
			texA.rgb*=1-shadowOnTexA;

			fixed shadowOnTexB=_ShadowStrength*(1-_Progress);
			shadowOnTexB *= (_Width*_Progress- posmod)/(_Width*_Progress);
			texB.rgb*=1-shadowOnTexB;
				
			
			if(posmod/_Width>_Progress){
					color=texA;
			}else{
					color=texB;
			}
			

//			fixed shadowOn=1;		
//			fixed strengtnWithProgress=_Progress/(1-_Progress);
//			fixed ratioInOneSide=(_Width*_Progress-posmod)/_Width;
//			if(posmod>_Progress*_Width){
//				shadowOn=1+_ShadowStrength*ratioInOneSide*strengtnWithProgress;
//			}else{				
//				shadowOn=1-_ShadowStrength*ratioInOneSide/strengtnWithProgress;
//			}						
//			color.rgb*=shadowOn;
			
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