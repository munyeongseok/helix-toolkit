#ifndef PSTERRAINSPLATVERTEXIDS_HLSL
#define PSTERRAINSPLATVERTEXIDS_HLSL
#define TERRAIN
#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

uint main(PSTerrainSplatVertexIDsInput input) : SV_Target
{
    return input.VertexID;
}

#endif