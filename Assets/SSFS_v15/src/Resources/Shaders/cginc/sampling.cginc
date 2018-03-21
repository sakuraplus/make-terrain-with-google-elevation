//	This is part of the Sinuous Sci-Fi Signs v1.5 package
//	Copyright (c) 2014-2018 Thomas Rasor
//	E-mail : thomas.ir.rasor@gmail.com

/*
This method samples the _Noise textured which is used for tile effect scattering.
*/
float noisetex(float2 uv)
{
#if ( COMPLEX )
	return Luminance(tex2D(_Noise, uv).rgb);
#else
	return tex2D(_Noise, uv).r;
#endif
}


/*
This method samples a texture 3 times to produce chromatic aberration based on a precomputed vector.
*/
#if ( ABERRATION )
half4 tex2DWithAberration( sampler2D tex, float2 uv, half2 aberrationVector , loosevars v )
{
	half4 col = lerp(_Color, _Color2, v.effect.base2 * _FlashAmount);

	float2 uvr = uv.xy + aberrationVector;
	float2 uvg = uv.xy;
	float2 uvb = uv.xy - aberrationVector;

	half r, g, b, a;

#if ( POST )
	// in post, _MainTex uses a zero alpha for the clearflags
	// because of this, we avoid using alpha in post so the skybox does not appear black
	r = (col.rgb * tex2D(tex, uvr).rgb).r;
	g = (col.rgb * tex2D(tex, uvg).rgb).g;
	b = (col.rgb * tex2D(tex, uvb).rgb).b;
	a = max(r, max(g, b));
#else
	half4 c = col * tex2D(tex, uvr);
	r = c.r * c.a;
	c = col * tex2D(tex, uvg);
	g = c.g * c.a;
	c = col * tex2D(tex, uvb);
	b = c.b * c.a;
	a = max(r, max(g, b));
#endif
	return half4(r, g, b, a);
}
#endif