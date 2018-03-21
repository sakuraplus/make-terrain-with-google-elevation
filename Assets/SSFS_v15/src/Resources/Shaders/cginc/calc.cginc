//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com


/*
Method for calculating various variables per pixel that will be used throughout the shader.
The loosevars struct is passed throughout the shader to maintain consistency 
and reduce unecessary or redundant computation.
*/
loosevars init_vars(fdata i)
{
	loosevars v;
	v.init();
	v.screenratio = _ScreenParams.y / _ScreenParams.x;
#ifdef SCREEN_POS
	v.scruv = i.scrpos.xy / i.scrpos.w;
#endif
	CalcTile(i.uv, v);
	CalcEffect( v );
#if (SCAN_LINES)
	CalcScanlines(i, v);
#endif
	CalcScaledUV(i.uv, v);
	CalcVisibility(v);
#if (CLIPPING)
	CalcTileClipping(v);
#endif
	CalcFlash(v);
#if (ABERRATION)
	CalcView(i, v);
#endif
	return v;
}


/*
Method for calculating tile information per pixel for later use.
*/
void CalcTile(float2 uv, inout loosevars v)
{
	float2 tc = _TileCount.xy;
	if (_SquareTiles > 0.5)
	{
#if ( POST )
		tc.y = tc.x * v.screenratio;
#else
		tc.y = tc.x;
#endif
	}

	v.tile.uv = floor(uv * tc.xy) / tc.xy;
	v.tile.width = 0.5 / tc.xy;
	v.tile.center = v.tile.uv + v.tile.width;
	v.tile.scatter = noisetex(v.tile.center) - 0.5;
}


/*
Method for calculating and storing the effect information for later use.
*/
void CalcEffect( inout loosevars v )
{
	phaseData pd = CalcPhaseData(v.tile.center, v.screenratio);
	CalcTransition(pd, v);
#if ( IDLE )
	CalcIdle(pd, v);
#endif

	v.effect.base = saturate(pow(1.0 - v.effect.transition, 3.0));
#if ( IDLE )
	v.effect.base = saturate(v.effect.base + v.effect.idle);
#endif
	v.effect.base2 = 2.0 * v.effect.base;
}


/*
Method for calculating the tile visibility including clipping and phase.
*/
void CalcVisibility( inout loosevars v )
{
	v.visibility = 1.0 - saturate(pow(1.0 - v.effect.transition, 4.0));
}


/*
Method for calculating tile clipping.
*/
#if (CLIPPING)
void CalcTileClipping( inout loosevars v )
{
	fixed scaleClip = 1.0;
	fixed2 clipFadeDist = 1.0 + v.tile.width;
	if (_RoundClipping)
	{
		float2 clipuv = v.scaledUv;
#if ( POST )
		clipuv -= v.tile.center;
		clipuv.y *= v.screenratio;
		clipuv += v.tile.center;
#endif
		scaleClip = smoothstep(clipFadeDist.x, 1.0, sqrthalf * length(clipuv - v.tile.center) / max(v.tile.width.x, v.tile.width.y));
	}
	else
	{
		fixed2 xyclip = abs(v.scaledUv - v.tile.center) / v.tile.width;
		fixed xclip = smoothstep(clipFadeDist.x, 1.0, xyclip.x);
		fixed yclip = smoothstep(clipFadeDist.y, 1.0, xyclip.y);
		scaleClip = xclip * yclip;
	}
	v.visibility = v.visibility * scaleClip;
}
#endif


/*
Method for calculating the effect flash of this tile.
*/
void CalcFlash( inout loosevars v ) { v.flash = 1.0 + v.effect.base * _FlashAmount * 10.0; }


/*
Method for computing view information for later use.
*/
#if ( ABERRATION )
void CalcView( fdata i , inout loosevars v )
{
#if ( POST )
	float2 scruv2 = saturate(v.scruv);
	scruv2 = scruv2 * 2.0 - 1.0;
	v.view.world = normalize(float3(scruv2.x, scruv2.y, 1.0));
	v.view.fresnel = 0.0;
#else
	v.view.world = normalize(UnityWorldSpaceViewDir(i.worldPos));
	CalcFresnel(i, v);
#endif
	v.view.local = mul(w2o, float4(v.view.world, 0.0)).xyz;
}


/*
Method for calculating the fresnel term of a pixel.
*/
void CalcFresnel(fdata i , inout loosevars v)
{
	v.view.fresnel = 1.0 - abs(dot(i.worldNormal, v.view.world));
}
#endif


/*
Method for calculating scanlines in the appropriate space.
*/
#if ( SCAN_LINES )
void CalcScanlines(fdata i , inout loosevars v)
{
	float scale = 10.0 * _ScanlineData.y + 1.0;
	float y = 0.0;
#ifdef SCREEN_POS
	y = (v.scruv.y * 0.02 * scale) * _ScreenParams.y;
#else
	y = i.worldPos.y * scale * 10.0;
#endif
	y += _Time.w * _ScanlineData.w;
	v.scanlines = 0.0;
#if (COMPLEX)
	fixed d = abs(2.0*frac(y) - 1.0) - 0.5;
	fixed f = fwidth(d);
	v.scanlines = (f != 0.0) ? smoothstep(-f, f, d) : d;
	v.scanlineShift = 5.0 * (v.scanlines-0.5) * (_ScanlineData.z / _ScreenParams.x);
#else
	v.scanlines = frac(y) < 0.5;
#endif
	v.scanlines = 1.0 - v.scanlines * _ScanlineData.x;
}
#endif


/*
Method for restricting the phase value to the range defined by the user.
*/
float CalcPhase( float phase , float start , float end )
{
	return saturate(invlerp(start, end, phase));
}


/*
Method for calculating a tile's position along the transition.
*/
float CalcPhasePos( float2 c , float screenratio )
{
	float2 phaseuv = c;

	float r = _PhaseDirection.x * tau;
	float sr = sin(r);
	float cr = cos(r);
	float pRange = max(abs(sr), abs(cr));
	phaseuv = mul(phaseuv - 0.5, float2x2(cr, sr, -sr, cr)) + 0.5;
	phaseuv = (phaseuv - 0.5) * pRange + 0.5;

	half phasePos = saturate(phaseuv.x);
#if (RADIAL)
	phaseuv = c;

#if (POST)
	phaseuv -= 0.5;
	phaseuv.y *= screenratio;
	phaseuv += 0.5;
#endif

	half radialPos = 1.0 - saturate(sqrthalf * (2.0 * length(phaseuv - 0.5)));
	phasePos = lerp(phasePos, radialPos, _PhaseDirection.y );
#endif
	if (_InvertPhase > 0.5)
		phasePos = 1.0 - phasePos;

	return phasePos;
}


/*
Method for calculating data for transition and idle animations.
*/
phaseData CalcPhaseData(float2 c , float screenratio )
{
	phaseData pd;
	pd.sharpness = _PhaseSharpness * 15.0 + 1.0;
	pd.invsharpness = 1.0 / pd.sharpness;
	pd.phasePos = CalcPhasePos(c , screenratio );
	pd.scatterAmount = _Scattering * 2.0;
	return pd;
}


/*
Method for calculating the base effect of tiles based on the _Phase property and the tile's position.
*/
void CalcTransition( phaseData pd , inout loosevars v )
{
	fixed p = _Phase;
	fixed q = 1.0 - p;
	half cleanupOffset = p - 0.5;
	half actualScatter = (v.tile.scatter + cleanupOffset) * pd.scatterAmount * pd.invsharpness;
	half n = pd.sharpness * (pd.phasePos + actualScatter - q) + q;
	v.effect.transition = saturate(n - (2.0 * q - 1.0));
}


/*
Method for calculating the idle animation effect of a tile.
*/
#if (IDLE)
void CalcIdle( phaseData pd , inout loosevars v )
{
	float time = _Time.y * _IdleData.y * 3.0 + v.tile.scatter * _IdleData.z  * _IdleData.z * 10.0;

	fixed p = frac(time);
	float idlePos = pd.phasePos;
	if (_IdleData.w > 0.5) idlePos = 1.0 - idlePos;
	fixed q = 1.0 - p;
	half cleanupOffset = p - 0.5;
	half actualScatter = (v.tile.scatter + cleanupOffset) * pd.scatterAmount * pd.invsharpness;
	half n = pd.sharpness * (idlePos + actualScatter - q) + q;
	v.effect.idle = _IdleData.x * (1.0 - saturate(abs(n - (3.0 * q - 1.0))));
}
#endif


/*
Method for calculating the UV as scaled per tile by effect.
*/
void CalcScaledUV(half2 texUv , inout loosevars v)
{
	half2 tl = v.tile.center - v.tile.width;
	half2 br = v.tile.center + v.tile.width;
	half2 center = 0.5;
#if (SCALE_AROUND_TILE)
	half2 tileCenterPoint = 0.0;
	tileCenterPoint.x = lerp(tl.x, br.x, _Scaling.z);
	tileCenterPoint.y = lerp(tl.y, br.y, _Scaling.w);
	center = lerp(saturate(_Scaling.zw), tileCenterPoint, _ScaleAroundTile);
#else
	center = saturate(_Scaling.zw);
#endif
	half2 scaling = v.effect.base * _Scaling.xy + 1.0;
	v.scaledUv = (texUv - center) * scaling + center;
}