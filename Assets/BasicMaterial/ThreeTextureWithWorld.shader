// *************Sa RTM*********

Shader "RTM/ThreeTextureWithWorld" {
		Properties {
		_MainTex("Map Base", 2D) = "white" {}
		_MapTexMax ("MapMax", 2D) = "white" {}
		_MapTexMin("MapMin", 2D) = "white" {}

		_heightMax("pos Max",Float) = 0
		_heightMin("pos Min",Float) = 0
		_transArea("Trans area",Float) = 0.2

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert


	struct Input
	{
		float3 worldPos;
		float2 uv_MainTex;
		float2 uv_MapTexMax;
	};



	float _heightMin;
	float _heightMax;
	float _transArea;

	sampler2D _MapTexMax;
	sampler2D _MainTex;
	sampler2D _MapTexMin;

	float remap(float value, float Vmin, float Vmax)
	{
			return (value-Vmin)/(Vmax-Vmin);
	}


	void surf (Input IN, inout SurfaceOutput o)
		  {
				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgba;

				float p;
				half heightCol=IN.worldPos.y;

				if(heightCol>=_heightMax){
					o.Albedo = tex2D (_MapTexMax, IN.uv_MapTexMax).rgba;
				}else if(heightCol<=_heightMin){
					o.Albedo = tex2D (_MapTexMin, IN.uv_MapTexMax).rgba;
				}
				else if( heightCol<(_heightMin+_transArea))
				{
						p =abs( remap(heightCol,_heightMin,_heightMin+_transArea));
						o.Albedo=p* tex2D (_MainTex, IN.uv_MainTex).rgb+(1-p)*tex2D (_MapTexMin, IN.uv_MapTexMax).rgb;
				}
				else if(heightCol>=(_heightMax-_transArea) )
				{
						p =abs( remap(heightCol,_heightMax-_transArea,_heightMax));

						o.Albedo=p* tex2D (_MapTexMax, IN.uv_MapTexMax).rgb+(1-p)*tex2D (_MainTex, IN.uv_MainTex).rgb;
				}

		  }

        ENDCG
	}
	}
