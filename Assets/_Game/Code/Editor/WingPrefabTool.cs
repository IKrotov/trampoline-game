using UnityEditor;
using UnityEngine;

namespace _Game.Code.Editor
{
    public static class WingPrefabTool
    {
        private const string PrefabFolder = "Assets/_Game/Prefabs/Wings";

        [MenuItem("GameObject/Save as Wing Prefab", false, 20)]
        static void SaveAsWingPrefab()
        {
            var selected = Selection.activeGameObject;
            if (selected == null)
            {
                EditorUtility.DisplayDialog("Wing Prefab Tool", "Select a wing object in the scene.", "OK");
                return;
            }

            string name = EditorInputDialog.Show("Save Wing Prefab", "Prefab name:", selected.name);
            if (string.IsNullOrEmpty(name)) return;

            string path = $"{PrefabFolder}/{name}.prefab";

            // Create empty root at world origin — local position will be (0,0,0) relative to any parent
            var root = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(root, "Create Wing Root");

            // Place root at parent's position so wing local position becomes the offset
            var parent = selected.transform.parent;
            if (parent != null)
            {
                root.transform.SetParent(parent);
                root.transform.localPosition = Vector3.zero;
                root.transform.localRotation = Quaternion.identity;
                root.transform.localScale = Vector3.one;
            }

            Undo.SetTransformParent(selected.transform, root.transform, "Reparent Wing");
            selected.transform.SetParent(root.transform, true); // worldPositionStays = true

            var prefab = PrefabUtility.SaveAsPrefabAssetAndConnect(root, path, InteractionMode.UserAction);

            if (prefab != null)
                Debug.Log($"[WingPrefabTool] Saved: {path}");
            else
                Debug.LogError($"[WingPrefabTool] Failed to save prefab at: {path}");
        }

        [MenuItem("GameObject/Save as Wing Prefab", true)]
        static bool SaveAsWingPrefabValidate() => Selection.activeGameObject != null;
    }

    // Minimal inline input dialog — avoids external dependencies
    public class EditorInputDialog : EditorWindow
    {
        private string _result;
        private bool _submitted;
        private System.Action<string> _callback;
        private string _label;

        public static string Show(string title, string label, string defaultValue = "")
        {
            // Synchronous: open as utility window and wait
            string result = null;
            var dialog = CreateInstance<EditorInputDialog>();
            dialog.titleContent = new GUIContent(title);
            dialog._label = label;
            dialog._result = defaultValue;
            dialog._callback = r => result = r;
            dialog.ShowModalUtility();
            return result;
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label(_label);
            GUI.SetNextControlName("input");
            _result = GUILayout.TextField(_result);
            EditorGUI.FocusTextInControl("input");
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Save") || (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return))
            {
                _callback?.Invoke(_result);
                Close();
            }
            if (GUILayout.Button("Cancel"))
            {
                _callback?.Invoke(null);
                Close();
            }
            GUILayout.EndHorizontal();
        }

        private void OnLostFocus() => Focus();
    }
}
