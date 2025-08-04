#ifndef VECTOR4_TO_TEXTURE_INCLUDED
#define VECTOR4_TO_TEXTURE_INCLUDED


void Vector4ToTexture_float(float4 color, float2 uv, out float4 result, out UnityTexture2D outputTexture)
{
    result = color; // La couleur est appliquée sur toute la "texture"
    outputTexture.tex = 0; // Faux retour de texture (ShaderGraph attend une UnityTexture2D)
}

#endif // VECTOR4_TO_TEXTURE_INCLUDED