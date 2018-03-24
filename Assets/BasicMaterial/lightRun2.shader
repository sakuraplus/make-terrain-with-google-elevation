// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "test/lightRun2" {
		Properties {
		_MainTex ("Main Texture", 2D) = "white" {}
		
		_FirstColor("First Color", Color) = (1, 0, 0, 1)
		_EndColor("End Color", Color) = (1, 0, 0, 1)
		_ColorSpeed("color speed", Float) = 1.0
		_ColorNum("color num", Float) = 1.0
				
		_MainAddup ("Addup", Range(0,1)) = 1.0
		_MaskTex ("Icon", 2D) = "white" {}
	}
	SubShader {
		CGINCLUDE
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;  
		float _MainAddup;
		sampler2D _MaskTex;  
		
		fixed4 _FirstColor;
		fixed4 _EndColor;
		float _ColorNum;
		float _ColorSpeed;
		
		uniform half4 _MainTex_ST;
		uniform half4 _MaskTex_ST;
	
	
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv: TEXCOORD0;
		};
		  
		v2f vert(appdata_img v) {
			v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);			


			o.uv=v.texcoord; ;//+ _MaskTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
			o.uv-=_MainTex_ST.zw;
			o.uv*=_MainTex_ST.xy;	
//			o.uv[1]=uv ;//+ _MaskTex_TexelSize.xy * half2(0*dir.x, 0*dir.y)*_RSize;
//			o.uv[1]-=_MaskTex_ST.zw;
//			o.uv[1]*=_MaskTex_ST.xy;		 
 
			return o;
		}
		
	
		
		fixed4 frag(v2f i) : SV_Target {
			
			fixed4 sum = tex2D(_MainTex, i.uv).rgba ;
			sum.rgb*=_MainAddup;
			
			fixed4 lightcolorStep=(_FirstColor-_EndColor)/_ColorNum;//
			
			int indcolor=floor(_ColorNum*(sin(_Time.y * _ColorSpeed)+1 )/2);

			fixed4 lightcolor	=_EndColor+lightcolorStep*indcolor;
				fixed4 mask = tex2D(_MaskTex, i.uv).rgba;
				if(mask.r>0){
					sum.rgb = lerp (sum.rgb, lightcolor.xyz, mask.rrr);
				//	sum.rgb+=lightcolor.rgb*mask.r; 

				}
			
				return sum;
		}
		    
		ENDCG
	
		
		Pass {
			
			//ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			  
			#pragma vertex vert  
			#pragma fragment frag
			#pragma shader_feature _hideIcon
			  
			ENDCG  
		}
		
		
	} 
	FallBack "Diffuse"
}