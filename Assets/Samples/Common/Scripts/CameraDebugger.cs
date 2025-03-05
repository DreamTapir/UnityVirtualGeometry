using UnityEngine;

namespace VirtualGeometry.Samples.Common
{
    public class CameraDebugger : MonoBehaviour
    {
        [SerializeField] private Camera _camera;

        private void OnDrawGizmos()
        {
            if (_camera == null)
                return;

            var color = Gizmos.color;
            var matrix = Gizmos.matrix;
            Gizmos.color = Color.red;
            Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, Vector3.one);

            if (_camera.orthographic)
            {
                var far = _camera.farClipPlane;
                var near = _camera.nearClipPlane;
                var spread = far - near;
                var center = (far + near) * 0.5f;
                var size = _camera.orthographicSize * 2;
                Gizmos.DrawWireCube(new Vector3(0, 0, center), new Vector3(size * _camera.aspect, size, spread));
            }
            else
            {
                Gizmos.DrawFrustum(Vector3.zero, _camera.fieldOfView, _camera.farClipPlane, _camera.nearClipPlane, _camera.aspect);
            }

            Gizmos.color = color;
            Gizmos.matrix = matrix;
        }
    }
}