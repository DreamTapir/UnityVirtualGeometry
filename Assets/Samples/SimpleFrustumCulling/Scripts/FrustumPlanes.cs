using System;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.Mathematics;

namespace VirtualGeometry.Samples.SimpleFrustumCulling
{
    public class FrustumPlaneBuffer : GPUBuffer<float4>
    {
        public FrustumPlaneBuffer(Camera camera)
        {
            var count = GeometryUtility.CalculateFrustumPlanes(camera).Length;

            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Structured, count, Marshal.SizeOf<float4>());
            _data = new float4[count];

            _buffer.SetData(_data);
        }

        public void SetPlanes(Camera camera)
        {
            var planes = GeometryUtility.CalculateFrustumPlanes(camera);

            // i is index of plane
            // 0 - left, 1 - right, 2 - bottom, 3 - top, 4 - near, 5 - far
            for (var i = 0; i < planes.Length; i++)
            {
                if (i >= _data.Length)
                {
                    break;
                }

                var plane = planes[i];
                _data[i].xyz = plane.normal;
                _data[i].w = plane.distance;
            }

            _buffer.SetData(_data);
        }
    }
}