//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

//properties available to any shader version
fixed4 _Color;
fixed4 _Color2;
sampler2D _MainTex;
float4 _MainTex_ST;
sampler2D _Noise;
float4 _TileCount;// [ count_x , count_y , z , w ]
float4 _Scaling;// [ scaling_x , scaling_y , origin_x , origin_y ]
float4 _PhaseDirection;// [ rotation , radial , z , w ]

fixed _Phase;
fixed _PhaseSharpness;
fixed _Scattering;
fixed _InvertPhase;
fixed _Overbright;
fixed _FlashAmount;
half _SquareTiles;

//properties tied to shader features
#if ( TEXTURE_SWAP ) 
sampler2D _MainTex2;
float4 _MainTex2_ST;
#endif

#if ( IDLE )
fixed4 _IdleData; // [ strength , speed , noise , reverse ]
#endif

#if ( ABERRATION ) 
fixed _Aberration, _EffectAberration;
#endif

#if ( CLIPPING )
fixed _ClippedTiles, _RoundClipping;
#endif

#if ( SCALE_AROUND_TILE )
fixed _ScaleAroundTile;
#endif

#if ( SCAN_LINES )
fixed4 _ScanlineData; // [ intensity , scale , uv_shift , speed ]
#define doScanlines ( _ScanlineIntensity.x > 0.02 )
#endif

#if ( COMPLEX )
fixed _BackfaceVisibility;
fixed _Flicker;
#endif


//assign debugcolor anywhere and then use "#define debug" to have the fragment program return it
float4 debugcolor;