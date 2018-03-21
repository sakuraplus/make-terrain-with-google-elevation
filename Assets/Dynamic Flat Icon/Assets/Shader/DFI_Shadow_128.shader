// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/FlatShadow_128"
{
  Properties {
    _MainTex ("Main Texture", 2D) = "white" {}
    _MainAddup ("Addup", Range(0,1)) = 1.0
    _IconTex ("Icon", 2D) = "white" {}
    _OffsetSize ("Offset", Range(0,2)) = 1.0
    _ShadowNum ("Shadow Length", Range(1,128)) = 1.0
    _ShadowRange ("Shadow Alpha", Range(0,1)) = 0.5
    _rot ("Rotation", Range(0,360)) = 1.0
    _decay ("Decay", Range(0.6,1)) = 1.0

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
    float _OffsetSize;
    float _ShadowNum;
    float _ShadowRange;
    fixed _rot;
  fixed _decay;
  fixed _Dirx;

  struct v2f {
    float4 pos : SV_POSITION;
    half2 uvmain:TEXCOORD0;
    half2 uvicon:TEXCOORD1;

  };


  v2f vert(appdata_img v)
    {
      v2f o;
			//o.pos = UnityObjectToClipPos(v.vertex);//mul(UNITY_MATRIX_MVP, v.vertex);
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			half2 uv = v.texcoord;
      o.uvmain=uv;
      o.uvicon=o.uvmain;

      return o;
    }


    fixed4 frag(v2f i) : SV_Target {

      fixed4 sum = tex2D(_MainTex, i.uvmain*_MainTex_ST-_MainTex_ST.zw).rgba;
      sum.rgb*=  _MainAddup;
      fixed4 sic = tex2D(_IconTex, i.uvicon*_IconTex_ST-_IconTex_ST.zw).rgba ;//* 0.5;
      _rot=fmod(_rot,360);
      _rot=radians(_rot);
      half2 dir=	(-1*half2(sin(_rot),-1*cos(_rot)));//normalize

      half2 uu=half2(0,0);
      fixed iconshadow=0;
	    for (int it = 127; it >=0 ; it--)
	      {
	        if(it>_ShadowNum-1 )
	        {
	          it=_ShadowNum-1;
	        }
	        int ii=it;
	    
	        uu= i.uvicon*_IconTex_ST-_IconTex_ST.zw + _IconTex_TexelSize.xy *half2(ii*dir.x,ii*dir.y)*_OffsetSize;
	
	        fixed shadowA = tex2D(_IconTex, uu).a ;
	        if(iconshadow<shadowA)
	        {
	          iconshadow=shadowA;
	        }
	        else
	        {
	          iconshadow*=_decay;
	        }
	      }//end for

      if(iconshadow>0)
      {
        float sr=lerp(1,1-_ShadowRange,iconshadow);
        sum.xyz *=sr;
      }

      sum.xyz = lerp (sum.xyz, sic.xyz, sic.www);
      return sum;
    }



    ENDCG


    Pass {
  
     //ZWrite Off
     Blend SrcAlpha OneMinusSrcAlpha
      CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

      ENDCG
    }



  }
  FallBack "Diffuse"
}
