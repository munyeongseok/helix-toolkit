#ifndef VSTERRAIN_HLSL
#define VSTERRAIN_HLSL

#define TERRAIN

#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"

#pragma pack_matrix(row_major)

GSTerrainInput main(VSTerrainInput input)
{
    GSTerrainInput output;
    
    float4 projPos = float4(input.Pos.xy, 0.5, 1);
    projPos.y *= -1;
    
    float3 chunkPos = float3(input.UV.xy, input.InstanceID * InvVoxelDimPlusMargins.x);
    chunkPos.xyz *= VoxelDim.x * InvVoxelDimMinusOne.x;
    
    float3 extChunkPos = (chunkPos * VoxelDimPlusMargins.x - Margin) * InvVoxelDim.x;
    float3 worldPos = WorldChunkPos + extChunkPos * WorldChunkSize;
 
    output.ProjPos = projPos;
    output.WorldPos = float4(worldPos, 1);
    output.InstanceID = input.InstanceID;
    output.ChunkPos = chunkPos;
    
    return output;
}

#endif