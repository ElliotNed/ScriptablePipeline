#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

float3 GetLighting(Surface surface, BRDF brdf, Light light)
{
    return IncomingLight(surface, light) * DirectBRDF(surface, brdf, light);
}

float3 GetLighting(Surface surface, BRDF brdf)
{
    float3 color;
    for (int i = 0; i < GetDirectionalLightCount(); ++i)
    {
        color += GetLighting(surface, brdf, GetDirectionalLight(i));
    }
    return color;
}

#endif