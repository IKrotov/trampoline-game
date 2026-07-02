using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class FenceWall : MonoBehaviour
{
    [SerializeField] private int count = 10;
    [SerializeField] private GameObject segmentPrefab;

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

        float sw = DetectWidth();
        if (sw <= 0f) { Debug.LogWarning("FenceWall: не удалось определить ширину сегмента"); return; }

        int n = Mathf.Max(1, count);
        for (int i = 0; i < n; i++)
        {
            var go = Instantiate(segmentPrefab, transform);
            go.name = $"Seg_{i}";
            go.transform.localPosition = new Vector3(sw * i, 0f, 0f);
            go.transform.localRotation = Quaternion.identity;
        }
    }

    private float DetectWidth()
    {
        var temp = Instantiate(segmentPrefab);
        float width = 0f;
        foreach (var r in temp.GetComponentsInChildren<MeshRenderer>())
            width = Mathf.Max(width, r.bounds.size.x);
        DestroyImmediate(temp);
        return width;
    }
}
