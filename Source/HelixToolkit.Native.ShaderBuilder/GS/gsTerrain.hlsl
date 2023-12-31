#ifndef GSTERRAIN_HLSL
#define GSTERRAIN_HLSL

#define TERRAIN

#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(3)]
void main(triangle GSTerrainInput input[3], inout TriangleStream<PSTerrainInput> outStream)
{
    for (int i = 0; i < 3; i++)
    {
        PSTerrainInput output;
        output.ProjPos = input[i].ProjPos;
        output.WorldPos = input[i].WorldPos;
        output.RTIndex = input[i].InstanceID;
        output.ChunkPos = input[i].ChunkPos;
        outStream.Append(output);
    }
    outStream.RestartStrip();
}

#endif