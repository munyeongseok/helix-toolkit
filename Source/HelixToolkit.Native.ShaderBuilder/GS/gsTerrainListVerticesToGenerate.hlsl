#ifndef GSTERRAINLISTNONEMPTYCELLS_HLSL
#define GSTERRAINLISTNONEMPTYCELLS_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(3)]
void main(point GSTerrainListVerticesToGenerateInput input[1], inout PointStream<PSTerrainListVerticesToGenerateInput> outStream)
{
    PSTerrainListVerticesToGenerateInput output;
    
    uint bits = input[0].Z8Y8X8Null5EdgeFlags3 & 0xFFFFFF00;
    
    if (input[0].Z8Y8X8Null5EdgeFlags3 & 1)
    {
        output.Z8Y8X8Null4EdgeNum4 = bits | 3;
        outStream.Append(output);
    }
    if (input[0].Z8Y8X8Null5EdgeFlags3 & 2)
    {
        output.Z8Y8X8Null4EdgeNum4 = bits | 0;
        outStream.Append(output);
    }
    if (input[0].Z8Y8X8Null5EdgeFlags3 & 4)
    {
        output.Z8Y8X8Null4EdgeNum4 = bits | 8;
        outStream.Append(output);
    }
}

#endif