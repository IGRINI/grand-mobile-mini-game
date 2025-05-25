using UnityEditor;
using UnityEngine;

namespace Game.Utils
{
    [ExecuteInEditMode]
    public class CaptureCubemap : MonoBehaviour
    {
        public int resolution = 128;
        public RenderTexture target;

        [ContextMenu("Capture")]
        public void Capture()
        {
            if (target == null)
            {
                target = new RenderTexture(resolution, resolution, 24);
                target.dimension = UnityEngine.Rendering.TextureDimension.Cube;
                target.hideFlags = HideFlags.HideAndDontSave;
            }

            Camera cam = GetComponent<Camera>();
            cam.RenderToCubemap(target);
        }
    }
}