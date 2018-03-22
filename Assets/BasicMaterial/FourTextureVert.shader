
// *************Sa RTM*********
Shader "RTM/fourTextureVert" {

	Properties {
		_MainTex("Map Base", 2D) = "white" {}
		_MapTexMax ("MapMax", 2D) = "white" {}
		_MapTexMin("MapMin", 2D) = "white" {}

		_heightMax("pos MAX",Float) = 1
		_heightMin("pos MIN",Float) = -1
		_transArea("Trans area",Range(0,1)) = 0.2


		[Header(Extra Texture 1)]
		[Toggle(_ExtraTexture01)] _ExtraTexture01("Extra Texture 01", float) = 1
		_MapTexCenter01("Extra Texture 01", 2D) = "white" {}
		_pos01("pos start",Range(0,1)) = -0.6
		_pos02("pos end",Range(0,1)) = -0.6

		//[HideIfDisabled(_ExtraTexture01)]
		[Header(Extra Texture 2)]
		[Toggle(_ExtraTexture02)] _ExtraTexture02("Extra Texture 02", float) = 1
		_MapTexCenter02("Extra Texture 02", 2D) = "white" {}
		_pos03("pos start",Range(0,1)) = -0.6
		_pos04("pos end",Range(0,1)) = -0.6







	}
	SubShader {
		Tags { "RenderType"="Opaque" }

		LOD 200

		CGPROGRAM
		#pragma surface surf Lambert vertex:vert
   	#pragma shader_feature _ExtraTexture01
   	#pragma shader_feature _ExtraTexture02


	struct Input
	{
		float2 indexT;
		float2 uv_MapTexMax;

	};


	sampler2D _MapTexMax;
	sampler2D _MainTex;
	sampler2D _MapTexMin;
	float _transArea;
	float _heightMax;
	float _heightMin;

	#ifdef _ExtraTexture02
		sampler2D _MapTexCenter02;
		float _pos04;
		float _pos03;
	#endif

	#ifdef _ExtraTexture01
		sampler2D _MapTexCenter01;
		float _pos02;
		float _pos01;
	#endif




//
	bool isin(float value,float p1,float p2){

			float minP=min(p1,p2);
			float maxP=max(p1,p2);

		if(value>=(minP) && value<=(maxP)){
				return true;
		}else{
			return false;//vpos;
		}
	}
//
	float remap(float value, float Vmin, float Vmax)
	{
			return (value-Vmin)/(Vmax-Vmin);
	}
	void surf (Input IN, inout SurfaceOutput o)
	{
			float pos=IN.indexT.y;
			float f;
			o.Albedo = tex2D (_MainTex, IN.uv_MapTexMax).rgb;//base texture

			//texture of up edge and down edge
			if(pos>=1 ||pos<=0){
				if(pos>=1){
					o.Albedo = tex2D (_MapTexMax, IN.uv_MapTexMax).rgb;
				}else {
					o.Albedo = tex2D (_MapTexMin, IN.uv_MapTexMax).rgb;
				}
			}else if(pos>=(1-_transArea) ||pos<=(0+_transArea)){
				if(pos>=(1-_transArea)){
					f=remap(pos,(1-_transArea),1);
					o.Albedo=f* tex2D (_MapTexMax, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MainTex, IN.uv_MapTexMax).rgb;
				}else {
					f=remap(pos,0,(0+_transArea));
					o.Albedo=f* tex2D (_MainTex, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MapTexMin, IN.uv_MapTexMax).rgb;
				}
			}

				#ifdef _ExtraTexture01
				if(pos>_pos01 && pos<_pos02){
						o.Albedo = tex2D (_MapTexCenter01, IN.uv_MapTexMax).rgb;
				}else if(pos>=_pos02&& pos<=_pos02+_transArea){
					f=remap(pos,_pos02,_pos02+_transArea);
					o.Albedo=f* tex2D (_MainTex, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MapTexCenter01, IN.uv_MapTexMax).rgb;
				}else if(pos<=_pos01&& pos>=_pos01-_transArea){
					f=remap(pos,_pos01-_transArea,_pos01);
					o.Albedo=f* tex2D (_MapTexCenter01, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MainTex, IN.uv_MapTexMax).rgb;
				}
			#endif

			#ifdef _ExtraTexture02
				if(pos>_pos03 && pos<_pos04){
						o.Albedo = tex2D (_MapTexCenter02, IN.uv_MapTexMax).rgb;
				}else if(pos>=_pos04&& pos<=_pos04+_transArea){
					f=remap(pos,_pos04,_pos04+_transArea);
					o.Albedo=f* tex2D (_MainTex, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MapTexCenter02, IN.uv_MapTexMax).rgb;
				}else if(pos<=_pos03&& pos>=_pos03-_transArea){
					f=remap(pos,_pos03-_transArea,_pos03);
					o.Albedo=f* tex2D (_MapTexCenter02, IN.uv_MapTexMax).rgb+(1-f)*tex2D (_MainTex, IN.uv_MapTexMax).rgb;
				}
			#endif
	}



	void vert (inout appdata_full v, out Input o)
	{
			UNITY_INITIALIZE_OUTPUT(Input,o);

			o.indexT.y=remap(v.vertex.y,_heightMin,_heightMax);

	}


        ENDCG
	}
	FallBack "Diffuse"
}
