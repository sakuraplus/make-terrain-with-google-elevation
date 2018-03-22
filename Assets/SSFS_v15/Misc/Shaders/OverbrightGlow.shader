Shader "Unlit/Overbright Glow"
{
	Properties
	{
		_Color("Color",Color) = (1.0,1.0,1.0,1.0)
		_MainTex ("Texture", 2D) = "white" {}
		_Overbright("Overbright" , Range(0.0,1.0)) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

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

			half4 _Color;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Overbright;
			
			v2f vert (appdata v)
			{
				v2f o;
//				o.vertex = UnityObjectToClipPos(v.vertex);
									o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//						o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				half4 col = tex2D(_MainTex, i.uv) * _Color;
				return col * (1.0 + _Overbright * 10.0);
			}
			ENDCG
		}
	}
}
