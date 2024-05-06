#ifndef GSTERRAINGENERATEVERTICES_HLSL
#define GSTERRAINGENERATEVERTICES_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(1)]
void main(point GSTerrainGenerateVerticesInput input[1], inout PointStream<PSTerrainGenerateVerticesInput> outStream)
{
    PSTerrainGenerateVerticesInput output;
    output.WorldPosAO = input[0].WorldPosAO;
    output.WorldNormal = float4(input[0].WorldNormal, 0);
    outStream.Append(output);
}

#endif