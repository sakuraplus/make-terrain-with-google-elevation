Shader "Custom/TunnelDemo_Standard" 
{
	Properties 
	{
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Glossiness ("Smoothness", Range(0,1)) = 0.5
		_Metallic ("Metallic", Range(0,1)) = 0.0
	}
	SubShader 
	{
		Tags { "RenderType"="Opaque" }
		
		CGPROGRAM
		#pragma surface surf Standard fullforwardshadows
		#pragma target 3.0

		sampler2D _MainTex;

		struct Input
		{
			float2 uv_MainTex;
			float3 worldPos;
		};

		half _Glossiness;
		half _Metallic;
		fixed4 _Color;

		uniform float _TunnelDemoDepth;
		uniform half4 _TunnelDemoFogColor;

		void surf (Input IN, inout SurfaceOutputStandard o)
		{
			float start = _TunnelDemoDepth - _TunnelDemoDepth * 0.6;
			float end = _TunnelDemoDepth;
			float fog = smoothstep(start, end, IN.worldPos.z);
			float fog2 = smoothstep(0.0, end, IN.worldPos.z);
			fog = fog * fog;
			o.Emission = 3.0 * _TunnelDemoFogColor.rgb * fog;

			fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
			o.Albedo = lerp(c.rgb, 1.0, fog2);
			o.Metallic = _Metallic;
			o.Smoothness = _Glossiness;
			o.Alpha = 1.0;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
