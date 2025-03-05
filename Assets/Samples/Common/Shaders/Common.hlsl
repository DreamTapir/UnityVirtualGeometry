#ifndef VIRTUAL_GEOMETRY_SAMPLE_COMMONE_HLSL
#define VIRTUAL_GEOMETRY_SAMPLE_COMMONE_HLSL

#include "Instance.hlsl"

float3 TransformPoint(float3 pos, float4x4 martix)
{
    float4 p = float4(pos, 1);
    p = mul(martix, p);
    p /= p.w;
    return p.xyz;
}

float3 TransformVector(float3 vec, float4x4 martix)
{
    float4 v = float4(vec, 0);
    v = mul(martix, v);
    return v.xyz;
}

float3 WorldPosToLocal(float3 pos, float4x4 martix)
{
    float3 uv = TransformPoint(pos, martix);
    return uv + 0.5f;
}

float3 LocalPosToWorld(float3 pos, float4x4 martix)
{
    float3 wPos = TransformPoint(pos - 0.5, martix);
    return wPos;
}

#endif // VIRTUAL_GEOMETRY_SAMPLE_COMMONE_HLSL