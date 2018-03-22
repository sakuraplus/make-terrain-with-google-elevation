Shader "RTM/ColorVertex"
{

		 Properties
    {
		[Toggle(_useAddup)] _useAddup("use color addup", float) = 1
      	_addup("addup", Range(0,1)) = 1
      	_addColor("addup", Color) = (1,1,1,1)
    }
     SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200
        
        CGPROGRAM

        #pragma surface surf Lambert
				#pragma shader_feature _useAddup
				
			float _addup;
			float4 _addColor;
        struct Input
        {
            float4 color: Color;
        };
				
        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = IN.color.rgb;
            #ifdef _useAddup
     				o.Albedo =(IN.color.rgb/2+_addColor.rgb)+0.5;
          	o.Albedo += _addup*IN.color.a;
          	#endif
        }

        ENDCG
    }
    FallBack "Diffuse"
}
