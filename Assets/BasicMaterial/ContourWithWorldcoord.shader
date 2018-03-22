// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// *************Sa RTM*********

Shader "RTM/ContourWithWorldcoord" {
		Properties {
		_MainTex ("Map Base", 2D) = "main" {}
		_colorSpace ("_colorSpace",Color) = (0,0,0,0)
		_colorLine ("_colorLine",Color) = (0,0,0,0)
		_space("space",Float) = 5
		_border("border",Float) = 1
		[Enum(multiply ,0,add,1)] _blendmode("Blend", int) = 0

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf CustomDiffuse
		//vertex:vert

        //Lambert
	inline float4 LightingCustomDiffuse (SurfaceOutput s, fixed3 lightDir, fixed atten) {
		float difLight = max(0, dot (s.Normal, lightDir));
		float4 col;
		col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
		col.a = s.Alpha;
		return col;
	}


	struct Input
	{
		float3 worldPos;
		float3 customColor;
		float2 uv_MainTex;
	};


	int _blendmode;
	sampler2D _MainTex;

	float4 _colorSpace;
	float4 _colorLine;

	float _space;
	float _border;

	void surf (Input IN, inout SurfaceOutput o)
		  {

				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgba;
				float p=IN.worldPos.y;


				float4 CC;

				if(p>=0){
					if(fmod(  p,_space)<_border )
						{
							CC=_colorLine;
						}
						else
						{
							CC=_colorSpace;
						}
					}else{
						if(fmod( abs( p),_space)<(_space- _border) )
						{
							CC=_colorSpace;
						}
						else
						{
							CC=_colorLine;
						}
					}
					CC*=CC.a;
					if(_blendmode==0){
						o.Albedo *=CC;
					}
					if(_blendmode==1){
						o.Albedo +=CC;
					}

		  }





        ENDCG
	}
	}
