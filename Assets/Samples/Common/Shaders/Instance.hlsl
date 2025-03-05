#ifndef VIRTUAL_GEOMETRY_SAMPLE_INSTANCE_HLSL
#define VIRTUAL_GEOMETRY_SAMPLE_INSTANCE_HLSL

struct Instance
{
    float4x4 localToWorld;
    float4 color;

    float3 GetPosition()
    {
		return localToWorld._m03_m13_m23;
	}
};

#endif // VIRTUAL_GEOMETRY_SAMPLE_INSTANCE_HLSL