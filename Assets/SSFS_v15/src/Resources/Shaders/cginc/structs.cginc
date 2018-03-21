//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

struct vdata
{
	float4 vertex : POSITION;
	float2 uv : TEXCOORD0;
#if ( COMPLEX )
	float4 color : COLOR;
#endif
#if ( ABERRATION )
	float3 normal : NORMAL;
#endif
};

struct fdata
{
	float4 vertex : SV_POSITION;
	float2 uv : TEXCOORD0;
	float2 texuv : TEXCOORD1;
#if ( COMPLEX )
	float4 color : COLOR;
#endif
#if ( ABERRATION || WORLD_SPACE_SCANLINES )
	float3 worldPos : TEXCOORD2;
#endif
#if ( ABERRATION )
	float3 normal : TEXCOORD3;
	float3 worldNormal : TEXCOORD4;
#endif
#ifdef SCREEN_POS
	float4 scrpos : TEXCOORD5;
#endif
};

struct tileData
{
	float2 uv;//bottom left corner of the current pixel's tile
	float2 width;//half width ( in uv space ) of each tile
	float2 center;//center of the current pixel's tile
	float2 scatter;//the amount of scatter for this tile

};

#if ( ABERRATION )
struct viewData
{
	fixed3 local;//local space view direction used for image aberration
	fixed3 world;//world space view direction used for fresnel
	fixed fresnel;//surface fresnel term cached
};
#endif

struct effectData
{
	float transition;//the transition state of this tile
#if ( IDLE )
	fixed idle;//the idle animation state of this tile
#endif

	fixed base;
	fixed base2;// base * 2.0
};

struct loosevars
{
	tileData tile;
#if ( ABERRATION )
	viewData view;
#endif
	effectData effect;
	float screenratio;
#ifdef SCREEN_POS
	float2 scruv;
#endif
#if ( SCAN_LINES )
	float scanlines;
	#if (COMPLEX)
		float scanlineShift;
	#endif
#endif
	half flash;
	float2 scaledUv;
	fixed visibility;

	void init()
	{
		tile.uv = 0.0;
		tile.width = 0.0;
		tile.center = 0.0;
		tile.scatter = 0.0;//[ -0.5 , +0.5 ]
#if ( ABERRATION )
		view.local = 0.0;
		view.world = 0.0;
		view.fresnel = 0.0;
#endif
		effect.transition = 0.0;
#if ( IDLE )
		effect.idle = 0.0;
#endif
		effect.base = 0.0;
		effect.base2 = 0.0;
#ifdef SCREEN_POS
		screenratio = 0.0;
		scruv = 0.0;
#endif
#if ( SCAN_LINES )
		scanlines = 0.0;
#if (COMPLEX)
		scanlineShift = 0.0;
#endif
#endif
		flash = 0.0;
		scaledUv = 0.0;
		visibility = 0.0;
	}
};

struct phaseData
{
	float sharpness;
	float invsharpness;
	float phasePos;
	float scatterAmount;
};