//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

float noisetex(float2 uv);
float2 rotuv(float2 uv, float r, float2 center = 0.5);
phaseData CalcPhaseData(float2 c, float screenratio);
void CalcTile(float2 uv, inout loosevars v);
void CalcScaledUV(half2 texUv, inout loosevars v);
float CalcPhasePos(float2 c,float screenratio);
void CalcVisibility(inout loosevars v);
void CalcFlash(inout loosevars v);
void CalcTileClipping(inout loosevars v);

void CalcTransition(phaseData pd, inout loosevars v);
void CalcEffect(inout loosevars v);
#if ( IDLE )
void CalcIdle(phaseData pd, inout loosevars v);
#endif

#if ( ABERRATION )
half2 FlattenVector(half3 v, half3 n);
void CalcFresnel(fdata i, inout loosevars v);
void CalcView(fdata i, inout loosevars v);
#endif

#if ( SCAN_LINES )
void CalcScanlines(fdata i, inout loosevars v);
#endif