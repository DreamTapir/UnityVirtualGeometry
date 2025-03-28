using UnityEngine;

namespace VirtualGeometry.Samples.Common
{
    public static class ShaderExtension
    {
        public static void SetKeyword(this Material material, string keyword, bool enabled)
        {
            if (enabled)
            {
                material.EnableKeyword(keyword);
            }
            else
            {
                material.DisableKeyword(keyword);
            }
        }

        public static void SetKeyword(this ComputeShader cs, string keyword, bool enabled)
        {
            if (enabled)
            {
                cs.DisableKeyword(keyword);
            }
            else
            {
                cs.EnableKeyword(keyword);
            }
        }
    }
}

