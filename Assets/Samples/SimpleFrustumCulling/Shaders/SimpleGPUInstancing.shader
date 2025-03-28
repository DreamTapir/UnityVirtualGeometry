Shader "VertualGeometrySample/SampleGPUInstancing"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ CULL_MESHLET
            #pragma multi_compile _ CULL_OCCLUSION
            #pragma multi_compile _ CULL_BACKFACE
            #pragma warning( disable: 3571 )
            #pragma enable_d3d11_debug_symbols

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/VirtualGeometry/Runtime/Shaders/Common.hlsl"
            #include "Assets/Samples/Common/Shaders/Common.hlsl"

            StructuredBuffer<uint> _Visibles;
            StructuredBuffer<Instance> _Instances;
            StructuredBuffer<Meshlet> _Meshlets;
            StructuredBuffer<Vertex> _Vertices;
            StructuredBuffer<float4> _Frustum;
            float3 _CameraPosition;

            struct v2g
            {
                uint instanceID : TEXCOORD;
            };

            struct g2f
			{
				float4 position : POSITION;
                float2 uv : TEXCOORD;
				half4 color : COLOR;
			};

            inline float3 ConvertHsvToRgb(float3 c)
            {
                const float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                const float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, clamp(p - K.xxx, 0.0, 1.0), c.y);
            }

            inline float GetNan() { return 0.0f / 0.0f; }

            v2g vert(uint instanceID : SV_INSTANCEID)
            {
                v2g o;
                o.instanceID = instanceID;
                return o;
            }

            void AppendToStream(inout TriangleStream<g2f> stream, const float3 positionWs, const float2 uv, float3 color)
            {
                g2f o;
                o.position = TransformWorldToHClip(positionWs);
                o.uv = uv;
                o.color = half4(color, 1);
                stream.Append(o);
            }

            [maxvertexcount(3)]
            void geom (triangle v2g input[3], inout TriangleStream<g2f> stream, uint primitiveID : SV_PRIMITIVEID)
            {
                const uint index = _Visibles[input[0].instanceID];
                const Instance i = _Instances[index];
                const Meshlet m = _Meshlets[primitiveID];
                const Light l = GetMainLight();

                // Get triangle vertices
                const Vertex v0 = _Vertices[m.indices.x];
                const Vertex v1 = _Vertices[m.indices.y];
                const Vertex v2 = _Vertices[m.indices.z];

                const float3 p0 = LocalPosToWorld(v0.position, i.localToWorld);
                const float3 p1 = LocalPosToWorld(v1.position, i.localToWorld);
                const float3 p2 = LocalPosToWorld(v2.position, i.localToWorld);

                const float3 facePosWS = (p0 + p1 + p2) / 3;
                const float3 faceNormal = normalize(cross(p1 - p0, p2 - p0));
                const float3 viewDir = normalize(_CameraPosition - facePosWS);

                // Culling per polygon
                const float3 min = LocalPosToWorld(m.aabbMin.xyz, i.localToWorld);
                const float3 max = LocalPosToWorld(m.aabbMax.xyz, i.localToWorld);
                // Frustum Culling & Occlusion Culling & Backface Culling
                bool visible = true;
#if defined(CULL_MESHLET)
                visible = visible && IsInFrustum(_Frustum, min, max);
#endif
#if defined(CULL_OCCLUSION)
#endif
#if defined(CULL_BACKFACE)
                visible = visible && dot(viewDir, faceNormal) > 0;
#endif
                const float nan = GetNan();

                // Calculate color
                const float3 color = i.color * l.color * l.shadowAttenuation * ConvertHsvToRgb(float3(primitiveID * 0.1, 1, 1)) * (dot(l.direction, faceNormal) * 0.5 + 0.5);

                AppendToStream(stream, visible ? p0 : nan, v0.uv, color);
                AppendToStream(stream, visible ? p1 : nan, v1.uv, color);
                AppendToStream(stream, visible ? p2 : nan, v2.uv, color);
                stream.RestartStrip();
            }

            half4 frag(g2f i) : COLOR
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}
