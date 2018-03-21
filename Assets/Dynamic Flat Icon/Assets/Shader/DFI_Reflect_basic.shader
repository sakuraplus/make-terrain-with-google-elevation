// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/Reflect_Basic"
{
  Properties {
    _MainTex ("Main Texture", 2D) = "white" {}
    _MainAddup ("Addup", Range(0,1)) = 1.0
    _IconTex ("Icon", 2D) = "white" {}

    _alphamax ("Max Alpha", Range(0,1)) = 1.0
    _ShadowPos ("Horizon", Range(0,1)) = 0.5
    _decay ("Decay", Range(0,1)) = 1.0
  }
  SubShader {
    CGINCLUDE

#include "UnityCG.cginc"

    sampler2D _MainTex;
    float _MainAddup;
    uniform half4 _MainTex_ST;
    uniform half4 _IconTex_ST;
    sampler2D _IconTex;
    half4 _IconTex_TexelSize;
 
    float _alphamax;
    float _ShadowPos;
	fixed _decay;

  struct v2f {
    float4 pos : SV_POSITION;

    half2 uvmain:TEXCOORD0;
    half2 uvicon:TEXCOORD1;
  };


  v2f vertBlurVertical(appdata_img v)
    {
      v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
      half2 uv = v.texcoord;
		o.uvmain=(uv+_MainTex_ST.zw)*_MainTex_ST;
		o.uvicon=(uv+_IconTex_ST.zw)*_IconTex_ST;
      return o;
    }




    fixed4 fragBlur(v2f i) : SV_Target {
       fixed4 sum = tex2D(_MainTex, i.uvmain).rgba;
      sum.rgb*=  _MainAddup;
      fixed4 sic;
      
     if(i.uvicon.y>_ShadowPos){
			sic = tex2D(_IconTex, i.uvicon).rgba ;//* 0.5;  
     }
     else
     {
   			
   		 half2 u2=half2(i.uvicon.x,_ShadowPos*2-i.uvicon.y);
         sic = tex2D(_IconTex,u2).rgba ;
         if(_decay<1){
	         half p=(_ShadowPos-i.uvicon.y)/(_ShadowPos*_decay);
	         p=clamp(p,0,1);
	         half alp=min(sic.a,_alphamax);
	      	
	         sic.a=lerp(alp,0,p);
 		}
     }

      sum.xyz = lerp (sum.xyz, sic.xyz, sic.aaa);
      return sum;
    }



    ENDCG


    Pass {

      //ZWrite Off
      Blend SrcAlpha OneMinusSrcAlpha
      CGPROGRAM

		#pragma vertex vertBlurVertical
		#pragma fragment fragBlur

      ENDCG
    }



  }
  FallBack "Diffuse"
}
