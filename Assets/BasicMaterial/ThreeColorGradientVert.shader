
// *************Sa RTM*********
Shader "RTM/ThreeColorGradientVert" {

	Properties {
		_MainTex ("Map Base", 2D) = "white" {}
		_colorUp ("Color Up",Color) = (0,0,0,1)
		_colorMid ("Color Mid",Color) = (0,0,0,1)
		_colorDown ("Color Down",Color) = (0,0,0,1)
		_heightMax("pos Max",Float) = 10
		_heightMid("pos Mid",Float) = 5
		_heightMin("pos Min",Float) = 0
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf CustomDiffuse vertex:vert

        //Lambert Standard CustomDiffuse
		inline float4 LightingCustomDiffuse (SurfaceOutput s, fixed3 lightDir, fixed atten) {
			float difLight = max(0, dot (s.Normal, lightDir));
			float4 col;
			col.rgb = s.Albedo * _LightColor0.rgb * (difLight * atten * 2);
			col.a = s.Alpha;
			return col;
		}


	struct Input
	{
		float3 customColor;
		float2 uv_MainTex;
	};



	sampler2D _MainTex;
	float4 _colorUp;
	float4 _colorDown;
	float4 _colorMid;
	float _heightMid;
	float _heightMax;
	float _heightMin;

	void surf (Input IN, inout SurfaceOutput o)
	{
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgba;
			o.Albedo *= IN.customColor;
	}




	float remap(float value, float Vmin, float Vmax)
	{
			return (value-Vmin)/(Vmax-Vmin);
	}

	void vert (inout appdata_full v, out Input o)
	{
			UNITY_INITIALIZE_OUTPUT(Input,o);
		 	half heightrange=_heightMax-_heightMin;

			half heightCol=(v.vertex.y-_heightMin)/(_heightMax-_heightMin);
			half heightColMid=(_heightMid-_heightMin)/(_heightMax-_heightMin);
			heightCol=clamp(heightCol,0,1);

			float4 CC;
			float p;

				if(heightCol>=0 &&heightCol<heightColMid)
					{
							p = remap(heightCol,0,heightColMid);
							CC =lerp(_colorDown,_colorMid, p);
					}
					else if(heightCol>=heightColMid &&heightCol<=1)
					{
							p = remap(heightCol,heightColMid,1);
							CC =lerp(_colorMid,_colorUp, p);
					}

		o.customColor=CC;
	}



        ENDCG
	}
	FallBack "Diffuse"
}
