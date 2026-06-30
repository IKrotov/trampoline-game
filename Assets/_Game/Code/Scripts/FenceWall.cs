using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FenceWall : MonoBehaviour
{
    [SerializeField] private float length = 25f;
    [SerializeField] private GameObject segmentPrefab;
    [Tooltip("Ширина одного сегмента. 0 = авто из меша")]
    [SerializeField] private float segmentWidth = 0f;

#if UNITY_EDITOR
    private bool _queued;
    private void OnValidate()
    {
        if (_queued) return;
        _queued = true;
        EditorApplication.delayCall += () =>
        {
            _queued = false;
            if (this == null) return;
            Rebuild();
        };
    }
#endif

    [ContextMenu("Rebuild")]
    public void Rebuild()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);

        if (segmentPrefab == null) return;

        float sw = segmentWidth > 0f ? segmentWidth : DetectWidth();
        if (sw <= 0f) { Debug.LogWarning("FenceWall: не удалось определить ширину сегмента"); return; }

        int count = Mathf.Max(1, Mathf.RoundToInt(length / sw));
        float spacing = length / count;

        for (int i = 0; i < count; i++)
        {
            var go = Instantiate(segmentPrefab, transform);
            go.name = $"Seg_{i}";
            go.transform.localPosition = new Vector3(spacing * i, 0f, 0f);
            go.transform.localRotation = Quaternion.identity;
        }
    }

    private float DetectWidth()
    {
        var mf = segmentPrefab.GetComponentInChildren<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return 0f;
        return mf.sharedMesh.bounds.size.x * mf.transform.lossyScale.x;
    }
}
