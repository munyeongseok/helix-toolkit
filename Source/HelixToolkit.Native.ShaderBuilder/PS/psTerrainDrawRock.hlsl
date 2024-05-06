#ifndef PSTERRAINDRAWROCK_HLSL
#define PSTERRAINDRAWROCK_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

float4 main(PSTerrainDrawRockInput input) : SV_Target
{
    float4 worldPosAO = input.WorldPosAO;
    
    /* Omit AO calculation
    float aoFactor = 1.0;
    float ao = saturate(lerp(0.5, worldPosAO.w, aoFactor) * 2.1 - 0.1);
    return float4(ao, 1);*/

    return float4(1, 0, 0, 1);
}

#endif