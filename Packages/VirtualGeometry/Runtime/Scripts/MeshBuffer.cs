using System;
using UnityEngine;

namespace VirtualGeometry
{
    public class MeshBuffer : IDisposable
    {
        private MeshletBuffer _meshlets;
        private VertexBuffer _vertices;

        public GraphicsBuffer Meshlets => _meshlets;
        public GraphicsBuffer Vertices => _vertices;

        public MeshBuffer(Mesh mesh)
        {
            _meshlets = new MeshletBuffer(mesh);
            _vertices = new VertexBuffer(mesh);
        }

        public void Dispose()
        {
            _meshlets?.Dispose();
            _vertices?.Dispose();
        }
    }
}

