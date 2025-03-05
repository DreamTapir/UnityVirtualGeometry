using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

namespace VirtualGeometry
{
    public struct Vertex
    {
        public float3 position; // 4bytes * 3 = 12bytes
        public float3 normal; // 4bytes * 3 = 12bytes
        public float2 uv; // 4bytes * 2 = 8bytes
    }

    public class VertexBuffer : GPUBuffer<Vertex>
    {
        public VertexBuffer(Mesh mesh)
        {
            var vertices = mesh.vertices;
            var normals = mesh.normals;
            var uvs = mesh.uv;
            var count = vertices.Length;

            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, Marshal.SizeOf<Vertex>());
            _buffer.name = $"{mesh.name}_Vertices";
            _data = new Vertex[count];

            for (int i = 0; i < count; i++)
            {
                var vertex = new Vertex();
                vertex.position = vertices[i];
                vertex.normal = normals[i];

                if (uvs.Length > 0 && count == uvs.Length)
                {
                    vertex.uv = uvs[i];
                }
                else
                {
                    vertex.uv = new float2(0, 0);
                }

                _data[i] = vertex;
            }

            _buffer.SetData(_data);
        }
    }
}

