#if UNITY_EDITOR
using UnityEditor;

namespace Coalballcat.Services.UI
{
    [CustomEditor(typeof(ResponsiveGridLayoutGroup))]
    [CanEditMultipleObjects]
    public class ResponsiveGridLayoutGroupEditor : Editor
    {
        // The built-in GridLayoutGroupEditor targets child classes too, which would hide
        // the extra responsive fields. Draw the full serialized object instead.
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif
