using System;
using UnityEngine;

namespace VirtualGeometry
{
    public abstract class AppendConsumeBuffer<T> : GPUBuffer<T>
    {
        protected GraphicsBuffer _counterBuffer;
        protected uint[] _counterData;

        public const int CounterSize = 5;
        public const int CounterStride = sizeof(uint);
        public const int CounterIndex = 1;
        public int CounterOffset = CounterStride * CounterIndex;
        public GraphicsBuffer Counter => _counterBuffer;

        public AppendConsumeBuffer()
        {
            _counterBuffer = new GraphicsBuffer(GraphicsBuffer.Target.IndirectArguments, CounterSize, CounterStride);
            _counterBuffer.name = "CounterBuffer";
            _counterData = new uint[CounterSize];

            for (uint i = 0; i < CounterSize; i++)
            {
                _counterData[i] = 0;
            }

            _counterBuffer.SetData(_counterData);
        }

        public virtual void CopyCount()
        {
            GraphicsBuffer.CopyCount(_buffer, _counterBuffer, CounterOffset);
        }

        public void SetCountData(uint[] counterData)
        {
            _counterData = counterData;
            _counterBuffer.SetData(_counterData);
        }

        public virtual void GetCountData(Array data)
        {
            _counterBuffer.GetData(data);
        }

        public override void Dispose()
        {
            base.Dispose();
            
            if (_counterBuffer != null)
            {
                _counterBuffer.Release();
                _counterBuffer = null;
            }
        }
    }
}

