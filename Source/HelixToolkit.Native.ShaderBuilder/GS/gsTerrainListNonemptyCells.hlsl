#ifndef GSTERRAINLISTNONEMPTYCELLS_HLSL
#define GSTERRAINLISTNONEMPTYCELLS_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(1)]
void main(point GSTerrainListNonemptyCellsInput input[1], inout PointStream<PSTerrainListNonemptyCellsInput> outStream)
{
    uint cubeCase = (input[0].Z8Y8X8CubeCase8 & 0xFF);
    if (cubeCase * (255 - cubeCase) > 0)
    {
        PSTerrainListNonemptyCellsInput output;
        output.Z8Y8X8CubeCase8 = input[0].Z8Y8X8CubeCase8;
        outStream.Append(output);
    }
}

#endif