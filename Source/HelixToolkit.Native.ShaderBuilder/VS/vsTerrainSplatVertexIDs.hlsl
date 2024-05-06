#ifndef VSTERRAINSPLATVERTEXIDS_HLSL
#define VSTERRAINSPLATVERTEXIDS_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

GSTerrainSplatVertexIDsInput main(VSTerrainSplatVertexIDsInput input, uint vertexID : SV_VertexID)
{
    uint edgeNum = input.Z8Y8X8Null4EdgeNum4 & 0x0F;
    int3 xyz = (int3)((input.Z8Y8X8Null4EdgeNum4.xxx >> uint3(8, 16, 24)) & 0xFF);
    
    xyz.x *= 3;
    if (edgeNum == 3)
        xyz.x += 0;
    if (edgeNum == 0)
        xyz.x += 1;
    if (edgeNum == 8)
        xyz.x += 2;

    float2 uv = (float2)xyz.xy;
    uv.x += 0.5 * InvVoxelDim.x / 3.0;
    uv.y += 0.5 * InvVoxelDim.x / 1.0;
    
    GSTerrainSplatVertexIDsInput output;
    output.ProjPos.x = (uv.x * InvVoxelDim.x / 3.0) * 2 - 1;
    output.ProjPos.y = (uv.y * InvVoxelDim.x) * 2 - 1;
    output.ProjPos.y *= -1;
    output.ProjPos.z = 0;
    output.ProjPos.w = 1;
    output.VertexIDAndSlice.x = vertexID;
    output.VertexIDAndSlice.y = xyz.z;

    return output;
}

#endif