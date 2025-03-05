using UnityEngine;
using Unity.Mathematics;
using VirtualGeometry.Samples.Common;

namespace VirtualGeometry.Samples.SimpleFrustumCulling
{
    public class SimpleFrustumCulling : MonoBehaviour
    {
        #region Serialize Fields
        [Header("Instance")]
        [SerializeField] private int _instanceCount = 1;
        [SerializeField] private float3 _offset = 0.0f;
        [SerializeField] private float3 _padding = 0.0f;
        [SerializeField] private float _scale = 1.0f;
        [SerializeField] private int _subMeshIndex = 0;
        [SerializeField] private Mesh _mesh;

        [Header("Camera")]
        [SerializeField] private Camera _camera;
        [SerializeField] private Bounds _bounds;

        [Header("Render")]
        [SerializeField] private ComputeShader _frustumCullingCs;
        [SerializeField] private Shader _render;
        #endregion

        #region Private Fields
        private Material _material;
        private Bounds _meshBounds;
        private uint[] _args = new uint[5] { 0, 0, 0, 0, 0 };

        private MeshBuffer _meshBuffer;
        private InstanceBuffer _instanceBuffer;
        private IndexBuffer _visibleBuffer;
        private FrustumPlaneBuffer _frustumPlaneBuffer;
        #endregion

        #region MonoBehaviour
        private void Start()
        {
            Init();
        }

        private void Update()
        {
            Culling();
            Render();
        }

        private void OnDestroy()
        {
            Release();
        }
        #endregion

        #region Private
        private void Init()
        {
            _material = new Material(_render);
            _meshBounds = _mesh.bounds;

            _meshBuffer = new MeshBuffer(_mesh);
            _instanceBuffer = new InstanceBuffer(_instanceCount, _offset, _padding, _scale);
            _visibleBuffer = new IndexBuffer(_instanceCount);
            _args[0] = (uint)_mesh.GetIndexCount(_subMeshIndex);
            _args[1] = (uint)_instanceCount;
            _visibleBuffer.SetCountData(_args);
            _frustumPlaneBuffer = new FrustumPlaneBuffer(_camera);
        }

        private void Culling()
        {
            var cs = _frustumCullingCs;
            var k = _frustumCullingCs.FindKernel("FrustumCulling");
            _visibleBuffer.Buffer.SetCounterValue(0);
            _frustumPlaneBuffer.SetPlanes(_camera);

            cs.SetInt("_InstanceCount", _instanceCount);
            cs.SetVector("_AabbMin", _meshBounds.min);
            cs.SetVector("_AabbMax", _meshBounds.max);
            cs.SetBuffer(k, "_Instances", _instanceBuffer);
            cs.SetBuffer(k, "_Visibles", _visibleBuffer);
            cs.SetBuffer(k, "_Meshlets", _meshBuffer.Meshlets);
            cs.SetBuffer(k, "_Vertices", _meshBuffer.Vertices);
            cs.SetBuffer(k, "_FrustumPlanes", _frustumPlaneBuffer);
            cs.GetKernelThreadGroupSizes(k, out uint x, out _, out _);
            cs.Dispatch(k, (int)Mathf.Max(1, (_instanceCount + x - 1) / x), 1, 1);
        }

        private void Render()
        {
            _visibleBuffer.CopyCount();
            _material.SetBuffer("_Visibles", _visibleBuffer);
            _material.SetBuffer("_Instances", _instanceBuffer);
            _material.SetBuffer("_Meshlets", _meshBuffer.Meshlets);
            _material.SetBuffer("_Vertices", _meshBuffer.Vertices);
            Graphics.DrawMeshInstancedIndirect(_mesh, _subMeshIndex, _material, _bounds, _visibleBuffer.Counter);
        }

        private void Release()
        {
            if (_material != null)
            {
                DestroyImmediate(_material);
                _material = null;
            }
            _meshBuffer?.Dispose();
            _instanceBuffer?.Dispose();
            _visibleBuffer?.Dispose();
            _frustumPlaneBuffer?.Dispose();
        }
        #endregion
    }
}

