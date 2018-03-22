Shader "RTM/TextureGraidentVert" {

	Properties {
		_MainTex ("Map Base", 2D) = "main" {}
		_GradientMap("Gradient Map", 2D) = "white" {}
		_heightMax("height Max",Float) = 5 	
		_heightMin("height Min",Float) = 1 

		[Toggle(_drawGrid)] _drawGrid("Draw Grid", float) = 1
		_colorLineOfGrid ("color Of Grid",Color) = (0,0,0,0)
		_spaceOfGrid("space",Float) = 5
		_borderOfGrid("border",Float) = 1

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        
		CGPROGRAM
		#pragma surface surf CustomDiffuse vertex:vert
    	#pragma shader_feature _drawGrid

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
		float2 uv_MainTex;
		float4 gradientColor;
		#ifdef _drawGrid
			float3 worldPos;
		#endif
	};


	sampler2D _GradientMap;
	sampler2D _MainTex;

	float _heightMax;
	float _heightMin;

	#ifdef _drawGrid
		float4 _colorLineOfGrid;
		float _spaceOfContor;
		float _border;
		float _spaceOfGrid;
		float _borderOfGrid;
	#endif
	
	void surf (Input IN, inout SurfaceOutput o)
	{
		
		o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
		o.Albedo *= IN.gradientColor;


		#ifdef _drawGrid
			float4 CC=(0,0,0,0);
			float px=IN.worldPos.x;
			float pz=IN.worldPos.z;
			if(pz>=0){
				if(fmod(  pz,_spaceOfGrid)<_borderOfGrid )
				{
					CC=_colorLineOfGrid;
				}
			}else{
				if(fmod( abs( pz),_spaceOfGrid)>=(_spaceOfGrid- _borderOfGrid) )
				{
					CC=_colorLineOfGrid;
				}
			}
						
			if(px>=0){
				if(fmod(  px,_spaceOfGrid)<_borderOfGrid )
				{
					CC=_colorLineOfGrid;
				}
			}else{
				if(fmod( abs( px),_spaceOfGrid)>=(_spaceOfGrid- _borderOfGrid) )
				{
					CC=_colorLineOfGrid;
				}
			}
			CC*=CC.a;
			o.Albedo+=CC;
		#endif

	}




	float remap(float value, float minSource, float maxSource)
	{
			return (value-minSource)/(maxSource-minSource);
	}
		
	void vert (inout appdata_full v, out Input o)
	{
			UNITY_INITIALIZE_OUTPUT(Input,o);
			float p=remap (v.vertex.y,_heightMax,_heightMin);
			p=clamp(p,0,1);
			o.gradientColor=tex2Dlod(_GradientMap, fixed4(p,0,0.5,0.5)).rgba;
	}


        ENDCG
	}
	FallBack "Diffuse"
}
