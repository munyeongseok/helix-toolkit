#ifndef PSTERRAIN_HLSL
#define PSTERRAIN_HLSL

#define TERRAIN
#define NOISE_LATTICE_SIZE 16
#define INV_LATTICE_SIZE (1.0/(float)(NOISE_LATTICE_SIZE))

#include"..\Common\CommonBuffers.hlsl"
#include"psCommon.hlsl"

float3 rot(float3 coord, float4x4 mat)
{
    return float3(dot(mat._11_12_13, coord),
                  dot(mat._21_22_23, coord),
                  dot(mat._31_32_33, coord));
}

float smooth_snap(float t, float m)
{
  // input: t in [0..1]
  // maps input to an output that goes from 0..1,
  // but spends most of its time at 0 or 1, except for
  // a quick, smooth jump from 0 to 1 around input values of 0.5.
  // the slope of the jump is roughly determined by 'm'.
  // note: 'm' shouldn't go over ~16 or so (precision breaks down).

  //float t1 =     pow((  t)*2, m)*0.5;
  //float t2 = 1 - pow((1-t)*2, m)*0.5;
  //return (t > 0.5) ? t2 : t1;
  
  // optimized:
    float c = (t > 0.5) ? 1 : 0;
    float s = 1 - c * 2;
    return c + s * pow(abs((c + s * t) * 2), m) * 0.5;
}

float4 NLQu(float3 uvw, Texture3D noiseTex)
{
    return noiseTex.SampleLevel(LinearRepeat, uvw, 0);
}

float4 NLQs(float3 uvw, Texture3D noiseTex)
{
    return NLQu(uvw, noiseTex) * 2 - 1;
}

float4 NMQu(float3 uvw, Texture3D noiseTex)
{
  // smooth the input coord
    float3 t = frac(uvw * NOISE_LATTICE_SIZE + 0.5);
    float3 t2 = (3 - 2 * t) * t * t;
    float3 uvw2 = uvw + (t2 - t) / (float) (NOISE_LATTICE_SIZE);
  // fetch
    return NLQu(uvw2, noiseTex);
}

float4 NMQs(float3 uvw, Texture3D noiseTex)
{
  // smooth the input coord
    float3 t = frac(uvw * NOISE_LATTICE_SIZE + 0.5);
    float3 t2 = (3 - 2 * t) * t * t;
    float3 uvw2 = uvw + (t2 - t) / (float) (NOISE_LATTICE_SIZE);
  // fetch  
    return NLQs(uvw2, noiseTex);
}

// SUPER MEGA HIGH QUALITY noise sampling (signed)
float NHQu(float3 uvw, Texture3D tex, float smooth = 1)
{
    float3 uvw2 = floor(uvw * NOISE_LATTICE_SIZE) * INV_LATTICE_SIZE;
    float3 t = (uvw - uvw2) * NOISE_LATTICE_SIZE;
    t = lerp(t, t * t * (3 - 2 * t), smooth);
 
    float2 d = float2(INV_LATTICE_SIZE, 0);

    // THE TWO-SAMPLE VERSION: much faster!
    // note: requires that three YZ-neighbor texels' original .x values
    //       are packed into .yzw values of each texel.
    float4 f1 = tex.SampleLevel(NearestRepeat, uvw2, 0).zxyw; // <+0, +y, +z, +yz>
    float4 f2 = tex.SampleLevel(NearestRepeat, uvw2 + d.xyy, 0).zxyw; // <+x, +xy, +xz, +xyz>
    float4 f3 = lerp(f1, f2, t.xxxx); // f3 = <+0, +y, +z, +yz>
    float2 f4 = lerp(f3.xy, f3.zw, t.yy); // f4 = <+0, +z>
    float f5 = lerp(f4.x, f4.y, t.z);
  
    return f5;
}

float NHQs(float3 uvw, Texture3D tex, float smooth = 1)
{
    return NHQu(uvw, tex, smooth) * 2 - 1;
}

float DENSITY(float3 ws)
{
  //-----------------------------------------------
  // This function determines the shape of the entire terrain.
  //-----------------------------------------------
 
  // Remember the original world-space coordinate, 
  // in case we want to use the un-prewarped coord.
  // (extreme pre-warp can introduce small error or jitter to
  //  ws, which, when magnified, looks bad - so in those
  //  cases it's better to use ws_orig.)
    float3 ws_orig = ws;
  
  // start our density value at zero.
  // think of the density value as the depth beneath the surface 
  // of the terrain; positive values are inside the terrain, and 
  // negative values are in open air.
    float density = 0;
  
  // sample an ultra-ultra-low-frequency (slowly-varying) float4 
  // noise value we can use to vary high-level terrain features 
  // over space.
    float4 uulf_rand = saturate(NMQu(ws * 0.000718, noiseVol0) * 2 - 0.5);
    float4 uulf_rand2 = NMQu(ws * 0.000632, noiseVol1);
    float4 uulf_rand3 = NMQu(ws * 0.000695, noiseVol2);
    
  //-----------------------------------------------
  // PRE-WARP the world-space coordinate.
    const float prewarp_str = 25; // recommended range: 5..25
    float3 ulf_rand = 0;
    ws += ulf_rand.xyz * prewarp_str * saturate(uulf_rand3.x * 1.4 - 0.3);
  //-----------------------------------------------
    
  // compute 8 randomly-rotated versions of 'ws'.  
  // we probably won't use them all, but they're here for experimentation.
  // (and if they're not used, the shader compiler will optimize them out.)
    float3 c0 = rot(ws, octaveMat0);
    float3 c1 = rot(ws, octaveMat1);
    float3 c2 = rot(ws, octaveMat2);
    float3 c3 = rot(ws, octaveMat3);
    float3 c4 = rot(ws, octaveMat4);
    float3 c5 = rot(ws, octaveMat5);
    float3 c6 = rot(ws, octaveMat6);
    float3 c7 = rot(ws, octaveMat7);

    // very general ground plane:
    density = -ws.y * 1;
    // to add a stricter ground plane further below:
    density += saturate((-4 - ws_orig.y * 0.3) * 3.0) * 40 * uulf_rand2.z;
    
#ifdef EVAL_CHEAP   //...used for fast long-range ambo queries
      float HFM = 0;
#else 
    float HFM = 1;
#endif
    
    // sample 9 octaves of noise, w/rotated ws coord for the last few.
    // note: sometimes you'll want to use NHQs (high-quality noise)
    //   instead of NMQs for the lowest 3 frequencies or so; otherwise
    //   they can introduce UNWANTED high-frequency noise (jitter).
    //   BE SURE TO PASS IN 'PackedNoiseVolX' instead of 'NoiseVolX'
    //   WHEN USING NHQs()!!!
    // note: higher frequencies (that don't matter for long-range
    //   ambo) should be modulated by HFM so the compiler optimizes
    //   them out when EVAL_CHEAP is #defined.
    // note: if you want to randomly rotate various octaves,
    //   feed c0..c7 (instead of ws) into the noise functions.
    //   This is especially good to do with the lowest frequency,
    //   so that it doesn't repeat (across the ground plane) as often...
    //   and so that you can actually randomize the terrain!
    //   Note that the shader compiler will skip generating any rotated
    //   coords (c0..c7) that are never used.
    density +=
           (0
             //+ NLQs(ws*0.3200*0.934, noiseVol3).x*0.16*1.20 * HFM // skipped for long-range ambo
             + NLQs(ws * 0.1600 * 1.021, noiseVol1).x * 0.32 * 1.16 * HFM // skipped for long-range ambo
             + NLQs(ws * 0.0800 * 0.985, noiseVol2).x * 0.64 * 1.12 * HFM // skipped for long-range ambo
             + NLQs(ws * 0.0400 * 1.051, noiseVol0).x * 1.28 * 1.08 * HFM // skipped for long-range ambo
             + NLQs(ws * 0.0200 * 1.020, noiseVol1).x * 2.56 * 1.04
             + NLQs(ws * 0.0100 * 0.968, noiseVol3).x * 5
             + NMQs(ws * 0.0050 * 0.994, noiseVol0).x * 10 * 1.0 // MQ
             + NMQs(c6 * 0.0025 * 1.045, noiseVol2).x * 20 * 0.9 // MQ
             + NHQs(c7 * 0.0012 * 0.972, packedNoiseVol3).x * 40 * 0.8 // HQ and *rotated*!
           );
            
  // LOD DENSITY BIAS:
  // this shrinks the lo- and medium-res chunks just a bit,
  // so that the hi-res chunks always "enclose" them:
  // (helps avoid LOD overdraw artifacts)
    density -= WorldChunkSize.x * 0.009;
        
    return density;
}

float main(PSTerrainInput input) : SV_Target
{
    return DENSITY(input.WorldPos.xyz);
}

#endif