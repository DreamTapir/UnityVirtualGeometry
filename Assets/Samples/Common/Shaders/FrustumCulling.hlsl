#ifndef FRUSTUM_CULLING_HLSL
#define FRUSTUM_CULLING_HLSL

static const int PLANE_NUM = 6;

inline bool IsInFrustum(StructuredBuffer<float4> planes, float3 min, float3 max)
{
    float3 pos = (min + max) * 0.5;
    
    float3 aabb[8];
    aabb[0] = float3(min.x, min.y, min.z);
    aabb[1] = float3(max.x, min.y, min.z);
    aabb[2] = float3(max.x, max.y, min.z);
    aabb[3] = float3(min.x, max.y, min.z);
    aabb[4] = float3(min.x, min.y, max.z);
    aabb[5] = float3(max.x, min.y, max.z);
    aabb[6] = float3(max.x, max.y, max.z);
    aabb[7] = float3(min.x, max.y, max.z);
    
    [unroll]
    for (int i = 0; i < PLANE_NUM; i++)
    {
        bool inside = false;
        const float3 normal = planes[i].xyz;
        const float pl_dist = planes[i].w;

        [unroll]
        for (int j = 0; j < 8; j++)
        {
            float pt_dist = dot(planes[i].xyz, aabb[j]) + pl_dist;

            if (0.0 <= pt_dist)
            {
                inside = true;
            }
        }

        if (!inside)
        {
            return false;
        }
    }
    
    return true;
}

#endif // FRUSTUM_CULLING_HLSL