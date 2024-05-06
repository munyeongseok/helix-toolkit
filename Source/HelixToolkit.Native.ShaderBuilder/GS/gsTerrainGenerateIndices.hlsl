#ifndef GSTERRAINBUILDDENSITY_HLSL
#define GSTERRAINBUILDDENSITY_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"..\Common\DataStructs.hlsl"

[maxvertexcount(15)]
void main(point GSTerrainGenerateIndicesInput input[1], inout TriangleStream<PSTerrainGenerateIndicesInput> outStream)
{
    uint cubeCase = input[0].Z8Y8X8CubeCase8 & 0xFF;
    uint numPolys = caseToNumPolys[cubeCase];
    int3 xyz = (int3)((input[0].Z8Y8X8CubeCase8.xxx >> uint3(8, 16, 24)) & 0xFF);

    for (uint i = 0; i < numPolys; i++)
    {
        int3 edgeNumsForTriangle = triTable[cubeCase * 5 + i].xyz;
        int3 xyzEdge;
        int3 vertexID;
        
        xyzEdge = xyz + (int3)edgeStart[edgeNumsForTriangle.x].xyz;
        xyzEdge.x = xyzEdge.x * 3 + edgeAxis[edgeNumsForTriangle.x].x;
        vertexID.x = texVertexIDVolume.Load(int4(xyzEdge, 0)).x;

        xyzEdge = xyz + (int3)edgeStart[edgeNumsForTriangle.y].xyz;
        xyzEdge.x = xyzEdge.x * 3 + edgeAxis[edgeNumsForTriangle.y].x;
        vertexID.y = texVertexIDVolume.Load(int4(xyzEdge, 0)).x;
        
        xyzEdge = xyz + (int3)edgeStart[edgeNumsForTriangle.z].xyz;
        xyzEdge.x = xyzEdge.x * 3 + edgeAxis[edgeNumsForTriangle.z].x;
        vertexID.z = texVertexIDVolume.Load(int4(xyzEdge, 0)).x;
        
        PSTerrainGenerateIndicesInput output;
        output.Index = vertexID.x;
        outStream.Append(output);
        output.Index = vertexID.y;
        outStream.Append(output);
        output.Index = vertexID.z;
        outStream.Append(output);
        outStream.RestartStrip();
    }
}

#endif