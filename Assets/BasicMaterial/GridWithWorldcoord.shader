// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// *************Sa RTM*********

Shader "RTM/GridWithWorldcoord" {
		Properties {
		_MainTex ("Map Base", 2D) = "main" {}
		[Enum(multiply ,0,add,1)] _blendmode("Blend", int) = 0
		
		_colorSpace ("color of Space",Color) = (0,0,0,0)

		
		[Toggle(_drawContour)] _drawContour("Draw contour", float) = 1
		_colorLineOfContour ("color Of Contour",Color) = (0,0,0,0)
		_spaceOfContour("space",Float) = 5
		_border("border",Float) = 1
		
		[Toggle(_drawGrid)] _drawGrid("Draw Grid", float) = 1
		_colorLineOfGrid ("color Of Grid",Color) = (0,0,0,0)
		_spaceOfGrid("space",Float) = 5
		_borderOfGrid("border",Float) = 1
		

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		#pragma surface surf CustomDiffuse
		#pragma shader_feature _drawContour
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
		float3 worldPos;
		float3 customColor;
		float2 uv_MainTex;
	};


	int _blendmode;
	sampler2D _MainTex;

	float4 _colorSpace;
	float4 _colorLineOfContour;
	float4 _colorLineOfGrid;
	float _spaceOfContour;
	float _border;
	float _spaceOfGrid;
	float _borderOfGrid;

	void surf (Input IN, inout SurfaceOutput o)
		  {

				o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgba;
				float p=IN.worldPos.y;
				
				float px=IN.worldPos.x;
				float pz=IN.worldPos.z;
				

				float4 CC;
				CC=_colorSpace;
				
				 #ifdef _drawContour
					if(p>=0){
						if(fmod(  p,_spaceOfContour)<_border )
							{
								CC=_colorLineOfContour;
							}
						}else{
							if(fmod( abs( p),_spaceOfContour)>=(_spaceOfContour- _border) )
							{
								CC=_colorLineOfContour;
							}
						}
					#endif
					
					
					#ifdef _drawGrid
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
					#endif
					
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
 