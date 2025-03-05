using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VirtualGeometry.Samples.Common
{
    public struct Instance
    {
        public float4x4 localToWorld;
        public float4 color;
    }

    public class InstanceBuffer : GPUBuffer<Instance>
    {
        public InstanceBuffer(int count, float3 offset, float3 padding, float scale)
        {
            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, Marshal.SizeOf<Instance>());
            _buffer.name = "InstanceBuffer";
            _data = new Instance[count];
            
            for (var i = 0; i < count; i++)
            {
                var data = new Instance();
                var position = new float3(offset.x + padding.x * i, offset.y + padding.y * i, offset.z + padding.z * i);
                var rotation = quaternion.EulerXYZ(new float3(Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f), Random.Range(0.0f, 360.0f)));
                data.localToWorld = float4x4.TRS(position, rotation, new float3(scale, scale, scale));
                var color = Random.ColorHSV();
                data.color = new float4(color.r, color.g, color.b, color.a);
                _data[i] = data;
            }
            _buffer.SetData(_data);
        }
    }
}