// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/MouseMoveHint"
{
	Properties
	{
		[HideInInspector]_MainTex("" , 2D) = "white"{}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" "PreviewType"="Plane"}
		Blend SrcAlpha OneMinusSrcAlpha
		ZWrite Off
		ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
					o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//						o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed easeinout(fixed x)
			{
				return 6.0 * pow(x, 5.0) - 15.0 * pow(x, 4.0) + 10.0 * pow(x, 3.0);
			}

			half4 frag (v2f i) : COLOR
			{
				fixed4 col = 0.0;
				
				fixed t = frac(0.15 * _Time.y );
				fixed p = max(0.0, 2.0 * t - 1.0);
				fixed c = max(0.0, 1.8 * t - 0.8) > 0.0;

				fixed r = easeinout(p) * 6.28318 * -3.0;

				fixed w = 0.05;

				fixed r1 = sin(r);
				fixed r2 = cos(r);
				fixed2 scaleduv = 3.0 * (i.uv - 0.5 + float2(r1,r2) * 0.05 );
				fixed2 cyluv = scaleduv;
				cyluv.y = max(0.0, abs(cyluv.y) - 0.35);
				fixed d = length(cyluv);
				col = smoothstep(0.5, 0.48, d) * smoothstep(0.48-w-0.02, 0.48-w, d);
				fixed inmouse = d < 0.475;
				fixed barh = scaleduv.y - 0.2;
				fixed barv = scaleduv.x;
				col += inmouse * smoothstep(0.0, 0.02, barh) * smoothstep(0.02+w+0.02, 0.02+w, barh);
				col += inmouse * smoothstep(0.5*w+0.02, 0.5*w, barv) * smoothstep(-0.5*w-0.02, -0.5*w, barv) * (scaleduv.y > 0.225);
				col += inmouse * (scaleduv.y > 0.225) * (scaleduv.x > 0.0) * 0.9 * c;
				return (saturate(col) * (0.9 + 0.05 * sin(_Time.y * 6.28318))) * saturate(saturate(sin(_Time.y * 0.2) - 0.1) + 0.1);
			}
			ENDCG
		}
	}
}
