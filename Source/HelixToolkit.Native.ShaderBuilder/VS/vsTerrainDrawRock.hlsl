#ifndef VSTERRAINBUILDDENSITY_HLSL
#define VSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

PSTerrainDrawRockInput main(VSTerrainDrawRockInput input)
{
    float4 worldPosAO = bufferWorldPosAO.Load(input.Index);
    float3 worldNormal = bufferWorldNormal.Load(input.Index).xyz;
    float4 projPos = mul(float4(worldPosAO.xyz, 1), mViewProjection);
    
    PSTerrainDrawRockInput output;
    output.Position = projPos;
    output.WorldPosAO = worldPosAO;
    output.WorldNormal = worldNormal;
    
    return output;
}

#endif