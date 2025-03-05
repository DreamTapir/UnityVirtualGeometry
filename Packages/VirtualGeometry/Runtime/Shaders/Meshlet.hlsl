#ifndef VIRTUAL_GEOMETRY_MESHLET_HLSL
#define VIRTUAL_GEOMETRY_MESHLET_HLSL

struct Meshlet
{
    float4 aabbMin;
    float4 aabbMax;
    int vertexOffset;
    int vertexCount;
};

#endif // VIRTUAL_GEOMETRY_MESHLET_HLSL