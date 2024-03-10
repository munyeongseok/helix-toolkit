#ifndef PSTERRAINBUILDDENSITY_HLSL
#define PSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

float4 SampleNoiseLowQualityUnsigned(float3 uvw, Texture3D noiseTexture)
{
    return noiseTexture.SampleLevel(samplerTerrainLinearReapeat, uvw, 0);
}

float4 SampleNoiseLowQualitySigned(float3 uvw, Texture3D noiseTexture)
{
    return SampleNoiseLowQualityUnsigned(uvw, noiseTexture) * 2 - 1;
}

float DENSITY(float3 wp)
{
    float density = 0;
    density = -wp.y * 1;
    density += SampleNoiseLowQualitySigned(wp * 0.0025 * 1.045, texTerrainNoiseVolume).x * 20 * 0.9;
    
    return density;
}

float main(PSTerrainBuildDensityInput input) : SV_Target
{
    return DENSITY(input.WorldPos.xyz);
}

#endif