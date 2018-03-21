//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

#define pi 3.14159
#define tau 6.28318
#define sqrthalf 0.707106781187

half2 FlattenVector(half3 v, half3 n)
{
	return (v - dot(v, n) * n).xy;
}

float2 rotuv(float2 p, float r, float2 c)
{
	r *= tau;
	float sr = sin(r);
	float cr = cos(r);
	return mul(p - c, float2x2(cr, sr, -sr, cr)) + c;
}

float invlerp(float a, float b, float x)
{
	return (x - a) / (b - a);
}