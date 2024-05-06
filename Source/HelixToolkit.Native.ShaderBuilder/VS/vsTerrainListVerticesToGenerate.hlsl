#ifndef VSTERRAINLISTVERTICESTOGENERATE_HLSL
#define VSTERRAINLISTVERTICESTOGENERATE_HLSL
#define TERRAIN
#include"..\Common\DataStructs.hlsl"
#include"..\Common\Common.hlsl"
#pragma pack_matrix(row_major)

GSTerrainListVerticesToGenerateInput main(VSTerrainListVerticesToGenerateInput input)
{
    int cubeCase = (int)(input.Z8Y8X8CubeCase8 & 0xFF);
    int bit0 = (cubeCase     ) & 1;
    int bit1 = (cubeCase >> 1) & 1;
    int bit3 = (cubeCase >> 3) & 1;
    int bit4 = (cubeCase >> 4) & 1;
    int3 buildVertexOnEdge = abs(int3(bit3, bit1, bit4) - bit0.xxx);

    uint bits = input.Z8Y8X8CubeCase8 & 0xFFFFFF00;
    if (buildVertexOnEdge.x != 0)
        bits |= 1;
    if (buildVertexOnEdge.y != 0)
        bits |= 2;
    if (buildVertexOnEdge.z != 0)
        bits |= 4;
    
    GSTerrainListVerticesToGenerateInput output;
    output.Z8Y8X8Null5EdgeFlags3 = bits;

    return output;
}

#endif