using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;

namespace VirtualGeometry
{
    /// <summary>
    /// A meshlet is a small mesh that is part of a larger mesh. It is used to optimize rendering by grouping vertices and indices together.
    /// data size: 16 + 16 + 4 + 4 = 40bytes
    /// </summary>
    public struct Meshlet
    {
        public float4 aabbMin; // 4bytes * 4 = 16bytes
        public float4 aabbMax; // 4bytes * 4 = 16bytes
        public int3 triangleID; // 4bytes
    }

    /// <summary>
    /// 
    /// </summary>
    public class MeshletBuffer : GPUBuffer<Meshlet>
    {
        public MeshletBuffer(Mesh mesh)
        {
            var triangles = mesh.triangles;
            var vertices = mesh.vertices;
            var count = triangles.Length / 3;

            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, Marshal.SizeOf<Meshlet>());
            _buffer.name = $"{mesh.name}_Meshlets";
            _data = new Meshlet[count];

            for (int i = 0; i < count; i++)
            {
                var index1 = triangles[i * 3];
                var index2 = triangles[i * 3 + 1];
                var index3 = triangles[i * 3 + 2];

                var v1 = vertices[index1];
                var v2 = vertices[index2];
                var v3 = vertices[index3];

                var meshlet = new Meshlet();
                meshlet.aabbMin = new float4(Vector3.Min(v1, Vector3.Min(v2, v3)), 0);
                meshlet.aabbMax = new float4(Vector3.Max(v1, Vector3.Max(v2, v3)), 0);
                meshlet.triangleID = new int3(index1, index2, index3);

                _data[i] = meshlet;
            }

            _buffer.SetData(_data);
        }
    }
}

