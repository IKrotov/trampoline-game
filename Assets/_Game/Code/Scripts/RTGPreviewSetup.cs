using UnityEngine;
using UnityEngine.Rendering.Universal;
using Object = UnityEngine.Object;

namespace _Game.Code.Scripts
{
    // Generates a Texture2D preview of a 3D model without using RTG.
    // Uses new RenderTexture() (not pooled) so no residual data from URP passes bleeds in.
    public static class ModelPreviewer
    {
        private const int PreviewLayer = 22;
        private static readonly Vector3 PreviewPos = new Vector3(-500f, -500f, -500f);
        private static readonly Vector3 CamDirection = new Vector3(-1f, -1f, -1f);

        public static Texture2D Generate(Transform model, int width, int height)
        {
            // Clone model to a hidden position on the preview layer
            var clone = Object.Instantiate(model.gameObject);
            clone.hideFlags = HideFlags.HideAndDontSave;
            SetLayerRecursive(clone, PreviewLayer);
            clone.transform.SetPositionAndRotation(PreviewPos, Quaternion.identity);
            clone.SetActive(true);

            var bounds = ComputeBounds(clone);
            if (!bounds.HasValue)
            {
                Object.DestroyImmediate(clone);
                return null;
            }

            var b = bounds.Value;

            // Fresh camera — no URP history, no accumulated post-processing state
            var camGO = new GameObject("[Preview Cam]") { hideFlags = HideFlags.HideAndDontSave };
            var cam = camGO.AddComponent<Camera>();
            cam.enabled = false;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.white;
            cam.cullingMask = 1 << PreviewLayer;
            cam.allowHDR = false;
            cam.allowMSAA = false;
            cam.aspect = (float)width / height;
            cam.nearClipPlane = 0.01f;

            // Disable URP post-processing entirely for this camera
            var urpData = camGO.GetComponent<UniversalAdditionalCameraData>()
                          ?? camGO.AddComponent<UniversalAdditionalCameraData>();
            urpData.renderPostProcessing = false;
            urpData.antialiasing = AntialiasingMode.None;

            // Frame the model
            var dir = CamDirection.normalized;
            float halfFov = cam.fieldOfView * 0.5f * Mathf.Deg2Rad;
            float radius = b.extents.magnitude;
            float dist = radius / Mathf.Sin(halfFov) * 1.2f;
            cam.transform.position = b.center - dir * dist;
            cam.transform.LookAt(b.center);
            cam.farClipPlane = dist + radius * 3f;

            // Fresh (non-pooled) render texture — never contains stale URP data
            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32);
            rt.antiAliasing = 1;
            rt.Create();

            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = null;

            // Read pixels
            var prevActive = RenderTexture.active;
            RenderTexture.active = rt;
            var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
            tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            tex.Apply();
            RenderTexture.active = prevActive;

            // Cleanup
            rt.Release();
            Object.DestroyImmediate(rt);
            Object.DestroyImmediate(camGO);
            Object.DestroyImmediate(clone);

            return tex;
        }

        private static void SetLayerRecursive(GameObject go, int layer)
        {
            go.layer = layer;
            foreach (Transform child in go.transform)
                SetLayerRecursive(child.gameObject, layer);
        }

        private static Bounds? ComputeBounds(GameObject go)
        {
            var renderers = go.GetComponentsInChildren<Renderer>();
            if (renderers.Length == 0) return null;
            var b = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) b.Encapsulate(renderers[i].bounds);
            return b;
        }
    }
}
