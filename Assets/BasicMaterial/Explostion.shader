// shader by Li Jiawei
// 2017/02/23
Shader "test/Seve/Explostion" {
	Properties {
		_Distance ("Distance", Range(0,10)) = 0
	}
	SubShader{
		Pass{
		Tags { "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
		// 声明程序
		#pragma vertex ver
		#pragma fragment frag
		#pragma geometry geo
		#pragma target 5.0
		#include "UnityCG.cginc"

		struct v2g {
			float4 pos : POSITION;
			float3 normal : NORMAL;
			float2 tex0 : TEXCOORD0;
		};

		struct g2f {
			float4 pos : POSITION;
			float2 tex0 : TEXCOORD0;
		};

		float rand(float3 co)
		{
			return frac(sin(dot(co.xyz, float3(12.9898, 78.233, 45.5432))) * 43758.5453);
		}

		//ver -> geo
		v2g ver(appdata_base v)
		{
			v2g output = (v2g)0;
			//output.pos = mul(unity_ObjectToWorld, v.vertex);//_Object2World
			output.pos = mul(_Object2World, v.vertex);//_Object2World
			output.normal = v.normal;
			output.tex0 = float2(0, 0);
			return output;
		}

		float _Distance;

		// geo -> fragment
		[maxvertexcount(4)]
		void geo(triangle v2g p[3], inout TriangleStream<g2f> triStream) //输出一个三角形stream,输入必须是一个数组？
		{
			float3 v1 = p[2].pos - p[0].pos;
			float3 v2 = p[1].pos - p[0].pos;
			float3 v_normal = normalize(cross(v1, v2));
			for (int i = 0; i < 3; ++i)
			{
				float3 v_pos = p[i].pos + v_normal * _Distance * rand(p[0].pos);
				float4 v = float4(v_pos, 1.0f);
				//float4x4 vp = mul(UNITY_MATRIX_MVP, unity_WorldToObject);//_Object2World
				float4x4 vp = mul(UNITY_MATRIX_MVP, _Object2World);//_Object2World
				g2f pIn;
				pIn.pos = mul(vp, v);
				pIn.tex0 = float2(1.0f, 1.0f);
				triStream.Append(pIn);
			}
		}

		//frag -> screen
		float4 frag(g2f input) : COLOR
		{
			//输出的颜色
			return float4(1.0f, 1.0f, 1.0f, 1.0f);
		}
		ENDCG
	}
	}
}