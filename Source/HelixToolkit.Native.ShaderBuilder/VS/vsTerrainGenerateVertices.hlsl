#ifndef VSTERRAINGENERATEVERTICES_HLSL
#define VSTERRAINGENERATEVERTICES_HLSL
#define TERRAIN
//#define AMBIENT_OCCLUSION_RAYS 32
//#define AMBIENT_OCCLUSION_STEPS 16
//#define GLOBAL_RAY_DIRECTION 
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

/* Omit AO calculation

struct Vertex
{
    float4 worldPosAO : POSITION;
    float3 worldNormalMisc : NORMAL;
};

Vertex PlaceVertexOnEdge(float3 worldPos_LL, float3 uvw_LL, int edgeNum)
{
    float density0 = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw_LL + InvVoxelDimPlusMarginsMinusOne.xxx * edgeStart[edgeNum], 0).x;
    float density1 = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw_LL + InvVoxelDimPlusMarginsMinusOne.xxx * edgeEnd[edgeNum], 0).x;
    float interpolatedDensity = saturate(density0 / (density0 - density1));
    
    float3 posWithinCell = edgeStart[edgeNum] + interpolatedDensity.xxx * edgeDir[edgeNum];
    float3 worldPos = worldPos_LL + posWithinCell * WorldVoxelSize.xxx;
    float3 uvw = uvw_LL + posWithinCell * InvVoxelDimPlusMarginsMinusOne.xxx;
    
    Vertex output;
    output.worldPosAO.xyz = worldPos.xyz;
    
    ...
}*/

GSTerrainGenerateVerticesInput main(VSTerrainGenerateVerticesInput input)
{
    uint3 unpackedPos;
    unpackedPos.x = (input.Z8Y8X8Null4EdgeNum4 >>  8) & 0xFF;
    unpackedPos.y = (input.Z8Y8X8Null4EdgeNum4 >> 16) & 0xFF;
    unpackedPos.z = (input.Z8Y8X8Null4EdgeNum4 >> 24) & 0xFF;
    float3 chunkPosWrite = (float3)unpackedPos * InvVoxelDimMinusOne.xxx;
    float3 chunkPosRead = (Margin + VoxelDimMinusOne * chunkPosWrite) * InvVoxelDimPlusMarginsMinusOne.xxx;
    float3 worldPos = WorldChunkPos + chunkPosWrite * WorldChunkSize;
    
    float3 uvw = chunkPosRead + InvVoxelDimPlusMarginsMinusOne.xxx * 0.25;
    uvw.xyz *= (VoxelDimPlusMargins.x - 1) * InvVoxelDimPlusMargins.x;

    /* Omit AO calculation
    
    int edgeNum = input.Z8Y8X8Null4EdgeNum4 * 0x0F;
    Vertex vertex = PlaceVertexOnEdge(worldPos, uvw, edgeNum);
    
    PSTerrainGenerateVerticesInput output;
    output.WorldPosAO = vertex.worldPosAO;
    output.WorldNormal = vertex.worldNormalMisc;*/
    
    GSTerrainGenerateVerticesInput output;
    output.WorldPosAO = float4(worldPos, 1);
    output.WorldNormal = worldPos;
    
    return output;
}

#endif