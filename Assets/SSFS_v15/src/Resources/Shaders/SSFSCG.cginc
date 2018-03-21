//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com
//
//	NOTE:
//	Do not delete this file.
//	This file contains the majority of the functionality for SSFS shaders.
//	If you do delete this file, SSFS shaders will no longer work and will fail to compile.

#define SSFSCGinc

//This looks like a comment... but it's not!
//Don't remove this, removal will prevent correct compilation for use with older Unity versions
//UNITY_SHADER_NO_UPGRADE

#if (UNITY_VERSION < 540)
#define o2w _Object2World
#define w2o _World2Object
#else
#define o2w unity_ObjectToWorld
#define w2o unity_WorldToObject
#endif

#if ( ( !WORLD_SPACE_SCANLINES && SCAN_LINES ) || POST )
#define SCREEN_POS
#endif

#include "UnityCG.cginc"
#include "cginc/maths.cginc"
#include "cginc/structs.cginc"
#include "cginc/vars.cginc"
#include "cginc/prototypes.cginc"
#include "cginc/sampling.cginc"
#include "cginc/calc.cginc"

fdata vert(vdata v)
{
	fdata o;
	o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//	o.vertex = UnityObjectToClipPos(v.vertex);
		//o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//	o.vertex = UnityObjectToClipPos(v.vertex);
	o.uv = v.uv;
	o.texuv = TRANSFORM_TEX(v.uv, _MainTex);
	#if ( COMPLEX )
		o.color = v.color;
	#endif
	#if ( ABERRATION )
		o.normal = v.normal;
		o.worldNormal = UnityObjectToWorldNormal(v.normal);
	#endif
	#if ( ABERRATION || WORLD_SPACE_SCANLINES )
		o.worldPos = mul(o2w, v.vertex).xyz;
	#endif
	#ifdef SCREEN_POS
		o.scrpos = ComputeScreenPos(o.vertex);
	#endif
	return o;
}

half4 frag
(
	fdata i
#if (COMPLEX && SHADER_TARGET > 45)
	// VFACE is only supported on shader model 4.x+ (using 4.5 for certainty)
	, fixed facing : VFACE
#endif
) : SV_TARGET
{
	loosevars v = init_vars(i);
	half4 col = 1.0;
	col = lerp(_Color, _Color2, v.effect.base * _FlashAmount);

	#if (ABERRATION)
		half2 aberrationBase = 0.0;
		half fresnelAberration = 0.0;
	#endif

	half4 tex1 = 0.0;
	
	#if(SCAN_LINES && COMPLEX)
		v.scaledUv.x += v.scanlineShift;
	#endif

	#if (POST)
		#if (ABERRATION)
			fresnelAberration = v.view.local.xy * 0.02 * _Aberration + v.view.local.xy * 0.02 * _EffectAberration * v.effect.base;
			#if(SCAN_LINES && COMPLEX)
				fresnelAberration += v.scanlineShift;
			#endif
			tex1 = tex2DWithAberration(_MainTex, v.scaledUv, fresnelAberration , v );
		#else
			tex1 = tex2D(_MainTex, v.scaledUv);
		#endif
	#else
		#if ( ABERRATION )
			aberrationBase = FlattenVector(v.view.world, i.worldNormal);
			fresnelAberration = v.view.fresnel * _Aberration;
			#if(SCAN_LINES && COMPLEX)
				fresnelAberration += v.scanlineShift;
			#endif
			half aberration1Strength = v.effect.base2 * _EffectAberration + fresnelAberration;
			half2 tex1aberration = aberrationBase * aberration1Strength;
			tex1 = tex2DWithAberration(_MainTex, v.scaledUv * _MainTex_ST.xy + _MainTex_ST.zw, tex1aberration * 0.025 , v );
		#else
			tex1 = tex2D(_MainTex, v.scaledUv * _MainTex_ST.xy + _MainTex_ST.zw);
		#endif
	#endif


	#if (TEXTURE_SWAP)
		half4 tex2 = 0.0;
		#if (ABERRATION)
			half aberration2Strength = (2.0 - v.effect.base2) * _EffectAberration + fresnelAberration;
			half2 tex2aberration = aberrationBase * aberration2Strength;
			tex2 = tex2DWithAberration(_MainTex2, i.uv * _MainTex2_ST.xy + _MainTex2_ST.zw, tex2aberration * 0.025 , v);
		#else
			tex2 = tex2D(_MainTex2, i.uv * _MainTex2_ST.xy + _MainTex2_ST.zw);
		#endif
		col *= lerp(tex2, tex1, v.visibility);
	#else
		col *= tex1 * v.visibility;
	#endif

	col.rgb *= (v.flash - 1.0) * v.visibility + 1.0;

	col.a = max(0.0,col.a);
	#if( !POST )
		col.rgb *= col.a;
	#endif
		col.rgb *= 1.0 + _Overbright*_Overbright * 16.0;
	#if (COMPLEX && !POST)
		col.rgb *= i.color.rgb;
	#endif
	#if (SCAN_LINES)
		col *= v.scanlines;
	#endif
	#if ( COMPLEX && !POST )
		col *= (1.0 - _Flicker * 0.2 *(floor(_Time.w*30.0) % 2.0 == 0.0));
		#if ( SHADER_TARGET > 45 )
			if (facing < 0.0) col = col * _BackfaceVisibility;
		#endif
	#endif

#ifdef debug
		return debugcolor;
#endif

	return col;
}