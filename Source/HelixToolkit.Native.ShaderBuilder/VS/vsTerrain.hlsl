#ifndef VSTERRAIN_HLSL
#define VSTERRAIN_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

#pragma pack_matrix(row_major)

GSTerrainInput main(VSTerrainInput input, uint instanceID : SV_InstanceID)
{
    GSTerrainInput output = (GSTerrainInput)0;
    
    float4 projPos = mul(input.Pos + float4(0, instanceID * 0.1, 0, 0), mViewProjection);
    //float4 projPos = float4(input.Pos.xy, 0.5, 1);
    //projPos.y *= -1;
    
    float3 chunkPos = float3(input.UV.xy, instanceID * InvVoxelDimPlusMargins.x);
    chunkPos.xyz *= VoxelDim.x * InvVoxelDimMinusOne.x;
    
    float3 extChunkPos = (chunkPos * VoxelDimPlusMargins.x - Margin) * InvVoxelDim.x;
    float3 worldPos = WorldChunkPos + extChunkPos * WorldChunkSize;
 
    output.ProjPos = projPos;
    output.WorldPos = float4(worldPos, 1);
    output.InstanceID = instanceID;
    output.ChunkPos = chunkPos;
    
    return output;
}

#endif