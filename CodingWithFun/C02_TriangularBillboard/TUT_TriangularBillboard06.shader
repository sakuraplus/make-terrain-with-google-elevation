// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'
//AnimationEffects2D-->sakuraplus-->https://sakuraplus.github.io/make-terrain-with-google-elevation/index.html
Shader "TUT/TriangularBillboard06_TypeB" {
	Properties {
		_MainTex ("Texture A", 2D) = "white" {}
		_MainTexB ("Texture B", 2D) = "black" {}
		_SpaceColor("Space Color", Color) = (0, 0, 0, 0)
		[Space(10)]
		_Progress("Progress", Range(0,1))=0
		_Width("Width",Float )=0.5
		_ShadowStrength("Shadow",  Range(0,2)) = 1.0
		_ScaleY("_ScaleY", Range(0.5,1))=0
	}


	SubShader {
		Tags {"Queue"="Transparent" }
		CGINCLUDE

		#include "UnityCG.cginc"

		sampler2D _MainTex;  
		sampler2D _MainTexB;  

		uniform half4 _MainTex_ST;
		uniform half4 _MainTexB_ST;
		half4 _SpaceColor;	
		fixed _Progress;
		float _Width;
		half _ShadowStrength;
		
		fixed _ScaleY;
		
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
			float	posmod= fmod(i.uvORI.x,_Width);
			
			if(i.uvORI.x>0.5){
				_Progress=0.5+0.5*_Progress;
			}else{
				_Progress=0.5-0.5*_Progress;	
			}
			
					
				fixed posfloor=(floor(_Progress/_Width)*_Width);					
				if(i.uvORI.x<posfloor){
					//grids prev
					if(i.uvORI.x<0.5){
						color=tex2D(_MainTex, i.uvTA).rgba;texB;
					}else{
						color= tex2D(_MainTexB, i.uvTB).rgba;//texA;
					}
				}else if(i.uvORI.x>(posfloor+_Width)){
					//grids next
					if(i.uvORI.x<0.5){
						color= tex2D(_MainTexB, i.uvTB).rgba;//texA;
					}else{
						color=tex2D(_MainTex, i.uvTA).rgba;//texB;
					}
				}else {

		
						float posTypeA=(_Progress-posfloor)/_Width;	
													
						fixed _ScaleYRight=_ScaleY+(1-posTypeA)*(1-_ScaleY);
						_ScaleYRight=_ScaleYRight+(_Width-posmod)*(1-_ScaleYRight)/(_Width*(1-posTypeA));					
						_ScaleYRight=0.5+(i.uvORI.y-0.5)/_ScaleYRight;
										
						fixed _ScaleYLeft=_ScaleY+(posTypeA)*(1-_ScaleY);	
						_ScaleYLeft=_ScaleYLeft+posmod*(1-_ScaleYLeft)/(_Width*posTypeA);					
						_ScaleYLeft=0.5+(i.uvORI.y-0.5)/_ScaleYLeft;		

						if(_ScaleYRight<=0 ||_ScaleYRight>=1 || _ScaleYLeft<=0 ||_ScaleYLeft>=1){
							return _SpaceColor;
						}								
						

						fixed2	uvA ;
						fixed2	uvB ;
						
						if(i.uvORI.x<0.5){
							uvB = fixed2(i.uvTB.x+(posmod-_Width)* posTypeA/(1-posTypeA), i.uvTB.y);
							uvA = fixed2(i.uvTA.x+(1-posTypeA)*posmod /posTypeA, i.uvTA.y);	
							
							i.uvTA.y=0.5+(i.uvTA.y-0.5)/_ScaleYRight;			
							i.uvTB.y=0.5+(i.uvTB.y-0.5)/_ScaleYLeft;	
							
							texA = tex2D(_MainTexB, uvB).rgba ;
							texB = tex2D(_MainTex, uvA).rgba ;	
						}else{						
							uvA = fixed2(i.uvTA.x+(posmod-_Width)* posTypeA/(1-posTypeA), i.uvTA.y);
							uvB = fixed2(i.uvTB.x+(1-posTypeA)*posmod /posTypeA, i.uvTB.y);
						
							i.uvTA.y=0.5+(i.uvTA.y-0.5)/_ScaleYLeft;			
							i.uvTB.y=0.5+(i.uvTB.y-0.5)/_ScaleYRight;								
							
							texA = tex2D(_MainTex, uvA).rgba ;
							texB = tex2D(_MainTexB, uvB).rgba ;							
						}			
						
	
						
						fixed shadowOnTexA=_ShadowStrength*posTypeA;
						shadowOnTexA *= (posmod-_Width*posTypeA)/(_Width*(1-posTypeA));
						texA.rgb*=1-shadowOnTexA;
			
						fixed shadowOnTexB=_ShadowStrength*(1-posTypeA);
						shadowOnTexB *= (_Width*posTypeA- posmod)/(_Width*posTypeA);
						texB.rgb*=1-shadowOnTexB;										

						if(i.uvORI.x>_Progress){
							color=texA;
						}else{
							color=texB;
						}

				}
			
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