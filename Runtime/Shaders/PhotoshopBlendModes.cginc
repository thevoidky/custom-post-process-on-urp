#ifndef PHOTOSHOP_BLENDMODES_INCLUDED
#define PHOTOSHOP_BLENDMODES_INCLUDED

//
// Ported from https://www.shadertoy.com/view/XdS3RW
//
// Original License:
//
// Creative Commons CC0 1.0 Universal (CC-0) 
//
// 25 of the layer blending modes from Photoshop.
//
// The ones I couldn't figure out are from Nvidia's advanced blend equations extension spec -
// http://www.opengl.org/registry/specs/NV/blend_equation_advanced.txt
// 
// ~bj.2013
//

// Helpers
#ifndef L_HELPER
#define L_HELPER half3(0.3, 0.59, 0.11)
#endif

/** @private */
float pinLight(float s, float d)
{
	return (2.0*s - 1.0 > d) ? 2.0*s - 1.0 : (s < 0.5 * d) ? 2.0*s : d;
}

/** @private */
float vividLight(float s, float d)
{
	return (s < 0.5) ? 1.0 - (1.0 - d) / (2.0 * s) : d / (2.0 * (1.0 - s));
}

/** @private */
float hardLight(float s, float d)
{
	return (s < 0.5) ? 2.0*s*d : 1.0 - 2.0*(1.0 - s)*(1.0 - d);
}

/** @private */
float softLight(float s, float d)
{
	return (s < 0.5) ? d - (1.0 - 2.0*s)*d*(1.0 - d)
		: (d < 0.25) ? d + (2.0*s - 1.0)*d*((16.0*d - 12.0)*d + 3.0)
		: d + (2.0*s - 1.0) * (sqrt(d) - d);
}

/** @private */
float overlay(float s, float d)
{
	return (d < 0.5) ? 2.0*s*d : 1.0 - 2.0*(1.0 - s)*(1.0 - d);
}

//    rgb<-->hsv functions by Sam Hocevar
//    http://lolengine.net/blog/2013/07/27/rgb-to-hsv-in-glsl
/** @private */
half3 rgb2hsv(half3 c)
{
	half4 K = half4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
	half4 p = lerp(half4(c.bg, K.wz), half4(c.gb, K.xy), step(c.b, c.g));
	half4 q = lerp(half4(p.xyw, c.r), half4(c.r, p.yzx), step(p.x, c.r));

	float d = q.x - min(q.w, q.y);
	float e = 1.0e-10;
	return half3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
}

/** @private */
half3 hsv2rgb(half3 c)
{
	half4 K = half4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
	half3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
	return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
}

half3 rgb2hsl(half3 color)
{
	half3 hsl; // init to 0 to avoid warnings ? (and reverse if + remove first part)

	float fmin = min(min(color.r, color.g), color.b);    //Min. value of RGB
	float fmax = max(max(color.r, color.g), color.b);    //Max. value of RGB
	float delta = fmax - fmin;             //Delta RGB value

	hsl.z = (fmax + fmin) / 2.0; // Luminance

	if (delta == 0.0)		//This is a gray, no chroma...
	{
		hsl.x = 0.0;	// Hue
		hsl.y = 0.0;	// Saturation
	}
	else                                    //Chromatic data...
	{
		if (hsl.z < 0.5)
			hsl.y = delta / (fmax + fmin); // Saturation
		else
			hsl.y = delta / (2.0 - fmax - fmin); // Saturation

		float deltaR = (((fmax - color.r) / 6.0) + (delta / 2.0)) / delta;
		float deltaG = (((fmax - color.g) / 6.0) + (delta / 2.0)) / delta;
		float deltaB = (((fmax - color.b) / 6.0) + (delta / 2.0)) / delta;

		if (color.r == fmax)
			hsl.x = deltaB - deltaG; // Hue
		else if (color.g == fmax)
			hsl.x = (1.0 / 3.0) + deltaR - deltaB; // Hue
		else if (color.b == fmax)
			hsl.x = (2.0 / 3.0) + deltaG - deltaR; // Hue

		if (hsl.x < 0.0)
			hsl.x += 1.0; // Hue
		else if (hsl.x > 1.0)
			hsl.x -= 1.0; // Hue
	}

	return hsl;
}

float hue2rgb(float f1, float f2, float hue)
{
	if (hue < 0.0)
		hue += 1.0;
	else if (hue > 1.0)
		hue -= 1.0;
	float res;
	if ((6.0 * hue) < 1.0)
		res = f1 + (f2 - f1) * 6.0 * hue;
	else if ((2.0 * hue) < 1.0)
		res = f2;
	else if ((3.0 * hue) < 2.0)
		res = f1 + (f2 - f1) * ((2.0 / 3.0) - hue) * 6.0;
	else
		res = f1;
	return res;
}

half3 hsl2rgb(half3 hsl)
{
	half3 rgb;

	if (hsl.y == 0.0)
		rgb.rgb = hsl.z; // Luminance
	else
	{
		float f2;

		if (hsl.z < 0.5)
			f2 = hsl.z * (1.0 + hsl.y);
		else
			f2 = (hsl.z + hsl.y) - (hsl.y * hsl.z);

		float f1 = 2.0 * hsl.z - f2;

		rgb.r = hue2rgb(f1, f2, hsl.x + (1.0 / 3.0));
		rgb.g = hue2rgb(f1, f2, hsl.x);
		rgb.b = hue2rgb(f1, f2, hsl.x - (1.0 / 3.0));
	}

	return rgb;
}

// Public API Blend Modes

half3 ColorBurn(half3 s, half3 d)
{
	return 1.0 - (1.0 - d) / s;
}

half3 LinearBurn(half3 s, half3 d)
{
	return s + d - 1.0;
}

half3 DarkerColor(half3 s, half3 d)
{
	return (s.x + s.y + s.z < d.x + d.y + d.z) ? s : d;
}

half3 Lighten(half3 s, half3 d)
{
	return max(s, d);
}

half3 Screen(half3 s, half3 d)
{
	return s + d - s * d;
}

half3 ColorDodge(half3 s, half3 d)
{
	return d / (1.0 - s);
}

half3 LinearDodge(half3 s, half3 d)
{
	return s + d;
}

half3 LighterColor(half3 s, half3 d)
{
	return (s.x + s.y + s.z > d.x + d.y + d.z) ? s : d;
}

half3 Overlay(half3 s, half3 d)
{
	half3 c;
	c.x = overlay(s.x, d.x);
	c.y = overlay(s.y, d.y);
	c.z = overlay(s.z, d.z);
	return c;
}

half3 SoftLight_(half3 s, half3 d)
{
	half3 c;
	c.x = softLight(s.x, d.x);
	c.y = softLight(s.y, d.y);
	c.z = softLight(s.z, d.z);
	return c;
}

half3 HardLight(half3 s, half3 d)
{
	half3 c;
	c.x = hardLight(s.x, d.x);
	c.y = hardLight(s.y, d.y);
	c.z = hardLight(s.z, d.z);
	return c;
}

half3 VividLight(half3 s, half3 d)
{
	half3 c;
	c.x = vividLight(s.x, d.x);
	c.y = vividLight(s.y, d.y);
	c.z = vividLight(s.z, d.z);
	return c;
}

half3 LinearLight(half3 s, half3 d)
{
	return 2.0*s + d - 1.0;
}

half3 PinLight(half3 s, half3 d)
{
	half3 c;
	c.x = pinLight(s.x, d.x);
	c.y = pinLight(s.y, d.y);
	c.z = pinLight(s.z, d.z);
	return c;
}

half3 HardMix(half3 s, half3 d)
{
	return floor(s + d);
}

half3 Difference(half3 s, half3 d)
{
	return abs(d - s);
}

half3 Exclusion(half3 s, half3 d)
{
	return s + d - 2.0*s*d;
}

half3 Subtract(half3 s, half3 d)
{
	return s - d;
}

half3 Divide(half3 s, half3 d)
{
	return s / d;
}

half3 Add(half3 s, half3 d)
{
	return s + d;
}

half3 Hue(half3 s, half3 d)
{
	d = rgb2hsv(d);
	d.x = rgb2hsv(s).x;
	return hsv2rgb(d);
}

half3 Luminosity(half3 s, half3 d)
{
	float dLum = dot(d, L_HELPER);
	float sLum = dot(s, L_HELPER);
	float lum = sLum - dLum;
	half3 c = d + lum;
	float minC = min(min(c.x, c.y), c.z);
	float maxC = max(max(c.x, c.y), c.z);
	if (minC < 0.0) return sLum + ((c - sLum) * sLum) / (sLum - minC);
	else if (maxC > 1.0) return sLum + ((c - sLum) * (1.0 - sLum)) / (maxC - sLum);
	else return c;
}

half3 Color(half3 s, half3 d)
{
	//s = rgb2hsv(s);
	////s.z = rgb2hsv(d).z;
	//s.z = dot(d, L_HELPER);
	////s.y *= (1.0f + s.z);
	//return hsv2rgb(s);
    return Luminosity(d, s);
}

half3 Saturation(half3 s, half3 d)
{
	d = rgb2hsv(d);
	d.y = rgb2hsv(s).y;
	return hsv2rgb(d);
}

#endif // PHOTOSHOP_BLENDMODES_INCLUDED
