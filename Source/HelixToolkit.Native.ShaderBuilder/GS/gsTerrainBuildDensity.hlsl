#ifndef GSTERRAINBUILDDENSITY_HLSL
#define GSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(3)]
void main(triangle GSTerrainBuildDensityInput input[3], inout TriangleStream<PSTerrainBuildDensityInput> outStream)
{
    for (int i = 0; i < 3; i++)
    {
        PSTerrainBuildDensityInput output;
        output.ProjPos = input[i].ProjPos;
        output.WorldPos = input[i].WorldPos;
        output.RTIndex = input[i].InstanceID;
        output.ChunkPos = input[i].ChunkPos;
        outStream.Append(output);
    }
    outStream.RestartStrip();
}

#endif