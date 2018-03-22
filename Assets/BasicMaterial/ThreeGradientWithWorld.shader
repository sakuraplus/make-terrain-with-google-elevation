// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// *************Sa RTM*********

Shader "RTM/ThreeGradientWithWorld" {
		Properties {
		_MainTex ("Map Base", 2D) = "xxx" {}
		_colorUp ("Color Up",Color) = (0,0,0,0)
		_colorMid ("Color Mid",Color) = (0,0,0,0)
		_colorDown ("Color Down",Color) = (0,0,0,0)
		_heightMax("pos Max",Float) = 0 
		_heightMid("pos Mid",Float) = 0 
		_heightMin("pos Min",Float) = 0 
		
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        
		CGPROGRAM
		#pragma surface surf CustomDiffuse 
     
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
		float2 uv_MainTex;
	};

	sampler2D _MainTex;
	float4 _colorUp;
	float4 _colorDown;
	float4 _colorMid;
	float _heightMid;
	float _heightMax;
	float _heightMin;


	float remap(float value, float Vmin, float Vmax)
	{
			return (value-Vmin)/(Vmax-Vmin);
	}
		

	void surf (Input IN, inout SurfaceOutput o) 
		  {
			o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgba;
			
				float p;

				half heightCol=(IN.worldPos.y-_heightMin)/(_heightMax-_heightMin);
				half heightColMid=(_heightMid-_heightMin)/(_heightMax-_heightMin);
				heightCol=clamp(heightCol,0,1);

				float4 CC;
				

				if(heightCol>=0 &&heightCol<heightColMid)
					{
							p = remap(heightCol,0,heightColMid);
							CC =lerp(_colorDown,_colorMid, p);
							// _colorMid * (heightCol-0)/(heightColMid-0) + _colorDown * (1- (heightCol-0)/(heightColMid-0)) ;
					}
					else if(heightCol>=heightColMid &&heightCol<=1)
					{
							p = remap(heightCol,heightColMid,1);
							CC =lerp(_colorMid,_colorUp, p);
							//CC = _colorUp * (heightCol-heightColMid)/(1-heightColMid) + _colorMid * (1-(heightCol-heightColMid)/(1-heightColMid)) ;
					}
					o.Albedo*=CC;			
				
		  }

        ENDCG
	}
	}
