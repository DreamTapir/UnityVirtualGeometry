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
            #pragma warning( disable: 3571 )
            #pragma enable_d3d11_debug_symbols

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/VirtualGeometry/Runtime/Shaders/Common.hlsl"
            #include "Assets/Samples/Common/Shaders/Common.hlsl"

            StructuredBuffer<uint> _Visibles;
            StructuredBuffer<Instance> _Instances;
            StructuredBuffer<Meshlet> _Meshlets;
            StructuredBuffer<Vertex> _Vertices;
            StructuredBuffer<float4> _Frustum;

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

            float3 ConvertHsvToRgb(float3 c)
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
                const Instance i = _Instances[_Visibles[input[0].instanceID]];
                const Meshlet m = _Meshlets[primitiveID];
                const Light l = GetMainLight();

                // Calculate triangle visivility
                const float3 min = LocalPosToWorld(m.aabbMin.xyz, i.localToWorld);
                const float3 max = LocalPosToWorld(m.aabbMax.xyz, i.localToWorld);
                const bool visible = IsInFrustum(_Frustum, min, max);
                const float4 nan = GetNan();

                const Vertex v0 = _Vertices[m.indices.x];
                const Vertex v1 = _Vertices[m.indices.y];
                const Vertex v2 = _Vertices[m.indices.z];

                const float d0 = dot(l.direction, TransformVector(v0.normal, i.localToWorld)) * 0.5 + 0.5;
                const float d1 = dot(l.direction, TransformVector(v1.normal, i.localToWorld)) * 0.5 + 0.5;
                const float d2 = dot(l.direction, TransformVector(v2.normal, i.localToWorld)) * 0.5 + 0.5;

                const float3 color = i.color * l.color * l.shadowAttenuation * ConvertHsvToRgb(float3(primitiveID * 0.1, 1, 1));

                AppendToStream(stream, visible ? LocalPosToWorld(v0.position, i.localToWorld) : nan, v0.uv, color * d0);
                AppendToStream(stream, visible ? LocalPosToWorld(v1.position, i.localToWorld) : nan, v1.uv, color * d1);
                AppendToStream(stream, visible ? LocalPosToWorld(v2.position, i.localToWorld) : nan, v2.uv, color * d2);
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
