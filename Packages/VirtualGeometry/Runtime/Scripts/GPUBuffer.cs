using System;
using UnityEngine;

public abstract class GPUBuffer<T> : IDisposable
{
    protected GraphicsBuffer _buffer;
    protected T[] _data;

    public int Count => _buffer.count;
    public int Stride => _buffer.stride;
    public GraphicsBuffer Buffer => _buffer;
    public T[] Data => _data;

    public static implicit operator GraphicsBuffer(GPUBuffer<T> buffer) => buffer._buffer;
    public static implicit operator T[](GPUBuffer<T> buffer) => buffer._data;

    public void SetData(T[] data)
    {
        _data = data;
        _buffer.SetData(_data);
    }

    public void GetData(T[] data)
    {
        _buffer.GetData(data);
    }

    public virtual void Dispose()
    {
        if (_buffer != null)
        {
            _buffer.Release();
            _buffer = null;
        }
    }
}
