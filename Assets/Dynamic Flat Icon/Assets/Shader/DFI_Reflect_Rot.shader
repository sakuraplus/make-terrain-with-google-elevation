// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Dynamic Flat Icon/Reflect_rot"
{
  Properties {
    _MainTex ("Main Texture", 2D) = "white" {}
    _MainAddup ("Addup", Range(0,1)) = 1.0
    _IconTex ("Icon", 2D) = "white" {}    
    _posx("Anchor x", Range(0,1)) = 0.5
    _posy("Anchor y", Range(0,1)) = 0.5
    _rot ("Rotation", Range(0,360)) = 1.0  
      
    _alphamax ("Max Alpha", Range(0,1)) = 1.0
    _ShadowLength ("Length", Range(0,2)) = 0.5
    _decay ("Decay", Range(0,2)) = 1.0
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
	float _ShadowLength;
	fixed _rot;
	fixed _posx;
  fixed _posy;
  fixed _decay;

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
      o.uvmain=(uv+_MainTex_ST.zw)*_MainTex_ST;
      o.uvicon=(uv+_IconTex_ST.zw)*_IconTex_ST;
      return o;
    }




   fixed4 frag(v2f i) : SV_Target {
      fixed4 sum = tex2D(_MainTex, i.uvmain).rgba;
      sum.rgb*= _MainAddup;
      fixed4 sic;

      float updown=i.uvicon.y;

			_rot=fmod(fmod(_rot,360)+360,360);
      if((_rot+90)%180==0) {
        _rot+=0.5;
      }
   
     	float ttan=tan(radians(_rot));
      float By=_posy-ttan*_posx+ttan*i.uvicon.x;
      if(((_rot<90 || _rot>270) && i.uvicon.y>By) || ((_rot>90 && _rot<270) &&i.uvicon.y<By) ) {
          sic = tex2D(_IconTex, i.uvicon).rgba ;//* 0.5;
          sum.xyz = lerp (sum.xyz, sic.xyz, sic.aaa);
        }else{
        	_rot=radians(_rot);
        	float H=cos(_rot)*sin(_rot) *(By-i.uvicon.y);
        	float W;
        	if(ttan==0){
        		W=(By-i.uvicon.y);
        	}else{
						W=H/ttan;
					}        	
        	float2 newxy=float2(i.uvicon.x-2*H,i.uvicon.y+2*W);
        	sic = tex2D(_IconTex, newxy).rgba ;
        	
        	float shadowA=min(_alphamax,sic.a);
        	float posA=(abs(H)+abs(W))/_ShadowLength-0.1;
        	
        	posA=clamp(posA*_decay,0,1);
 
        	float sica=lerp(shadowA,0,posA);
        	sic.a*=sica;
        	sum.xyz = lerp (sum.xyz, sic.xyz, sic.aaa);
       		
       }      
     
      return sum;
    }



    ENDCG


    Pass {

    //ZWrite off
    Blend SrcAlpha OneMinusSrcAlpha
      CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

      ENDCG
    }



  }
  FallBack "Diffuse"
}
