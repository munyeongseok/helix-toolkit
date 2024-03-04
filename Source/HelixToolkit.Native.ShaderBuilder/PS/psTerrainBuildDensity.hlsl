#ifndef PSTERRAINBUILDDENSITY_HLSL
#define PSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

float4 NLQu(float3 uvw, Texture3D noiseTex)
{
    return noiseTex.SampleLevel(LinearRepeat, uvw, 0);
}

float4 NLQs(float3 uvw, Texture3D noiseTex)
{
    return NLQu(uvw, noiseTex) * 2 - 1;
}

float DENSITY(float3 ws)
{
    float density = 0;
    density = -ws.y * 1;
    density += NLQs(ws * 0.0025 * 1.045, noiseVol0).x * 20 * 0.9;
    
    return density;
}

float main(PSTerrainBuildDensityInput input) : SV_Target
{
    return DENSITY(input.WorldPos.xyz);
}

#endif