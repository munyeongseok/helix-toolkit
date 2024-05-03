#ifndef VSTERRAINLISTNONEMPTYCELLS_HLSL
#define VSTERRAINLISTNONEMPTYCELLS_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

GSTerrainListNonemptyCellsInput main(VSTerrainListNonemptyCellsInput input, uint instanceID : SV_InstanceID)
{
    float3 chunkCoordRead = float3(input.UVRead.x, input.UVRead.y, (instanceID + Margin) + InvVoxelDimPlusMargins.x);
    float3 chunkCoordWrite = float3(input.UVWrite.x, input.UVWrite.y, instanceID * InvVoxelDim.x);
    
    float3 uvw = chunkCoordRead + InvVoxelDimPlusMarginsMinusOne.xxx * 0.125;
    uvw.xy *= ((VoxelDimPlusMargins.x - 1) * InvVoxelDimPlusMargins.x).xx;
    
    float4 field0123;
    float4 field4567;
    field0123.x = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.yyy, 0).x;
    field0123.y = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.yxy, 0).x;
    field0123.z = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.xxy, 0).x;
    field0123.w = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.xyy, 0).x;
    field4567.x = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.yyx, 0).x;
    field4567.y = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.yxx, 0).x;
    field4567.z = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.xxx, 0).x;
    field4567.w = texTerrainDensityVolume.SampleLevel(samplerTerrainNearestClamp, uvw + InvVoxelDimPlusMarginsMinusOne.xyx, 0).x;

    uint4 i0123 = (uint4) saturate(field0123 * 99999);
    uint4 i4567 = (uint4) saturate(field4567 * 99999);
    int cubeCase = (i0123.x     ) | (i0123.y << 1) | (i0123.z << 2) | (i0123.w << 3) |
                   (i4567.x << 4) | (i4567.y << 5) | (i4567.z << 6) | (i4567.w << 7);
    
    uint3 coord3 = uint3(input.UVWrite.xy * VoxelDimMinusOne.xx, instanceID);
    
    GSTerrainListNonemptyCellsInput output;
    output.Z8Y8X8CubeCase8 = (coord3.z << 24) |
                             (coord3.y << 16) |
                             (coord3.x <<  8) |
                             (cubeCase      );
    return output;
}

#endif