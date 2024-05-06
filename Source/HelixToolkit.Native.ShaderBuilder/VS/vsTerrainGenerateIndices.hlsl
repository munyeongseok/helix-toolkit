#ifndef VSTERRAINBUILDDENSITY_HLSL
#define VSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

GSTerrainGenerateIndicesInput main(VSTerrainGenerateIndicesInput input)
{
    GSTerrainGenerateIndicesInput output;
    output.Z8Y8X8CubeCase8 = input.Z8Y8X8CubeCase8;
    return output;
}

#endif