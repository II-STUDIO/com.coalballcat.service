#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Coalballcat.Services
{
    [CustomPropertyDrawer(typeof(LayerAttribute))]
    public class LayerAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType == SerializedPropertyType.Integer)
            {
                property.intValue = EditorGUI.LayerField(position, label, property.intValue);
            }
            else
            {
                EditorGUI.LabelField(position, label.text, "Use [Layer] on an int field");
            }
        }
    }
}
#endif
