using UnityEngine;

namespace VirtualGeometry
{
    public class IndexBuffer : AppendConsumeBuffer<uint>
    {
        public IndexBuffer(int count) : base()
        {
            _buffer = new GraphicsBuffer(GraphicsBuffer.Target.Append, count, sizeof(uint));
            _buffer.name = "IndexBuffer";
            _data = new uint[count];

            for (uint i = 0; i < count; i++)
            {
                _data[i] = 0;
            }

            _buffer.SetData(_data);
        }
    }
}