Shader "RTM/TwoTextureVert" {

	Properties {
		_MapTexUp ("Map Up", 2D) = "white" {}
		_MainTex("Map Down", 2D) = "white" {}
	
		_heightMax("pos Max",Float) = 1 	
		_heightMin("pos Min",Float) = -1 

	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200
        
		CGPROGRAM
		#pragma surface surf Lambert vertex:vert

	struct Input
	{
		float indexT;
		float2 uv_MapTexUp;
	};


	sampler2D _MainTex;
	sampler2D _MapTexUp;

	float _heightMax;
	float _heightMin;
	
	
	void surf (Input IN, inout SurfaceOutput o)
	{
		if(IN.indexT==1){
		o.Albedo = tex2D (_MapTexUp, IN.uv_MapTexUp).rgb;
		}else if(IN.indexT==0){
		o.Albedo = tex2D (_MainTex, IN.uv_MapTexUp).rgb;
		}else{
			o.Albedo=IN.indexT* tex2D (_MapTexUp, IN.uv_MapTexUp).rgb+(1-IN.indexT)*tex2D (_MainTex, IN.uv_MapTexUp).rgb;
		}
	}




	float remap(float value, float Vmin, float Vmax)
	{
			return (value-Vmin)/(Vmax-Vmin);
	}
		
	void vert (inout appdata_full v, out Input o)
	{
			UNITY_INITIALIZE_OUTPUT(Input,o);
				float p=remap (v.vertex.y,_heightMin,_heightMax);
				p=clamp(p,0,1);
			
				o.indexT=p;
	}


        ENDCG
	}
	FallBack "Diffuse"
}
