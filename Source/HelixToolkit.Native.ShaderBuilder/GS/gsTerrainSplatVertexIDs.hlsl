#ifndef GSTERRAINBUILDDENSITY_HLSL
#define GSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(1)]
void main(point GSTerrainSplatVertexIDsInput input[1], inout PointStream<PSTerrainSplatVertexIDsInput> outStream)
{
    PSTerrainSplatVertexIDsInput output;
    output.ProjPos = input[0].ProjPos;
    output.VertexID = input[0].VertexIDAndSlice.x;
    output.RTIndex = input[0].VertexIDAndSlice.y;
    outStream.Append(output);
}

#endif