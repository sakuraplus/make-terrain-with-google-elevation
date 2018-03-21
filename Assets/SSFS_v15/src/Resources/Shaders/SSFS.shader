//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

Shader "Sci-Fi/SSFS/Base"
{
	Properties
	{
		/*
			This property block is ugly but necessary for Material Property
			validation in the Custom Material Inspector.
			
			NOTE: See "vars.cginc" for variable component explanations.
		*/
		
		[HideInInspector]_BlendSrc("Blend Src" , Int) = 1
		[HideInInspector]_BlendDst("Blend Dst" , Int) = 0
		[HideInInspector]_Cull("" , Int) = 2
		[HideInInspector]_ZWrite("" , Int) = 8
		[HideInInspector]_ZTest("" , Int) = 0

		_MainTex("", 2D) = "white" {}
		[HideInInspector]_MainTex2("",2D) = "black"{}

		_Color("" , Color) = (1.0,1.0,1.0,1.0)
		_Color2 ("" , Color) = (1.0,1.0,1.0,1.0)

		_Overbright("",Range(0.0,1.0))=0.25
		[NoScaleOffset]_Noise ("", 2D) = "gray" {}
		_TileCount("" , Vector) = (25.0,25.0,0.0,0.0)
		_SquareTiles("" , Float) = 0.0
		_Phase("" , Range(0.0 , 1.0)) = 1.0
		[Toggle]_InvertPhase("" , Float) = 0.0

		_IdleData("",Vector) = (0.1,0.1,0.0,0.0)

		_PhaseDirection("" , Vector) = (0.0,0.0,0.0,0.0)
		_PhaseSharpness("" , Range(0.0,1.0)) = 0.5
		_Scattering("" , Float) = 0.25
		_Scaling("",Vector) = (1.0,1.0,0.5,0.5)
		_Aberration("" , Range(0.0,1.0)) = 0.5
		_EffectAberration("" , Range(0.0,1.0)) = 0.5
		_FlashAmount("" , Range(0.0,1.0)) = 0.5
		_Flicker("" , Range(0.0,0.2)) = 0.1
		_BackfaceVisibility("" , Range(0.0,1.0)) = 1.0

		_ScanlineData("" , Vector) = (0.2,0.5,0.0,0.0)
		_ScaleAroundTile("" , Float) = 1.0
		[Toggle]_ClippedTiles("" , Float) = 1.0
		[Toggle]_RoundClipping("" , Float) = 0.0
	}

	SubShader
	{
		Tags { "Queue" = "Transparent" "PreviewType" = "Plane" }
		ZWrite [_ZWrite]
		ZTest [_ZTest]
		Blend [_BlendSrc] [_BlendDst]
		Cull [_Cull]

		Pass
		{
			CGPROGRAM
			/*
				This is the list of possible shader feature keywords.
				Set these per material to have that material use specific features.
				Keywords are set automatically when using the included Editors.
				One downside to shader_feature is that new shader variants cannot be created from script
				and all Keyword variants that are not used by materials at compilation time are excluded.
				On the flip side, multi_compile compiles ALL keyword variants at compilation time.
				This means when this shader is compiled, it may have quite a few versions (~1024?)
			*/

			/*
			#pragma shader_feature ABERRATION
			#pragma shader_feature COMPLEX
			#pragma shader_feature CLIPPING
			#pragma shader_feature IDLE
			#pragma shader_feature POST
			#pragma shader_feature RADIAL
			#pragma shader_feature SCALE_AROUND_TILE
			#pragma shader_feature SCAN_LINES
			#pragma shader_feature TEXTURE_SWAP
			#pragma shader_feature WORLD_SPACE_SCANLINES
			*/

			#pragma multi_compile __ ABERRATION
			#pragma multi_compile __ COMPLEX
			#pragma multi_compile __ CLIPPING
			#pragma multi_compile __ IDLE
			#pragma multi_compile __ POST
			#pragma multi_compile __ RADIAL
			#pragma multi_compile __ SCALE_AROUND_TILE
			#pragma multi_compile __ SCAN_LINES
			#pragma multi_compile __ TEXTURE_SWAP
			#pragma multi_compile __ WORLD_SPACE_SCANLINES

			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			#include "SSFSCG.cginc"
			ENDCG
		}
	}
	CustomEditor "SSFS_Editor"
}