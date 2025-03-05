Shader "VertualGeometrySample/SampleGPUInstancing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/VirtualGeometry/Runtime/Shaders/Common.hlsl"
            #include "Assets/Samples/Common/Shaders/Common.hlsl"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            StructuredBuffer<uint> _Visibles;
            StructuredBuffer<Instance> _Instances;
            StructuredBuffer<Meshlet> _Meshlets;
            StructuredBuffer<Vertex> _Vertices;

            struct v2f
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                half4 color : COLOR;
            };

            v2f vert(uint id : SV_VertexID, uint instanceID: SV_InstanceID)
            {
                const uint index = _Visibles[instanceID];
                const Instance i = _Instances[index];
                const Vertex v = _Vertices[id];
                const Light l = GetMainLight();

                const half4 lightColor = half4(l.color, 1) * (dot(l.direction, TransformVector(v.normal, i.localToWorld)) * 0.5 + 0.5) * l.shadowAttenuation;

                v2f o = (v2f)0;
                o.vertex = TransformObjectToHClip(LocalPosToWorld(v.position, i.localToWorld));
                o.uv = v.uv;
                o.color = (half4)i.color * lightColor;
                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                return i.color;
            }
            ENDHLSL
        }
    }
}
