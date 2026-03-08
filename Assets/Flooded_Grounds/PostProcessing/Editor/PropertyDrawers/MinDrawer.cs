using UnityEngine;
using UnityEditor;

namespace UnityEditor.PostProcessing
{
    [CustomPropertyDrawer(typeof(UnityEngine.PostProcessing.MinAttribute))]
    sealed class MinDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // Hata buradaydı: Hangi MinAttribute olduğunu açıkça belirtiyoruz
            var attr = (UnityEngine.PostProcessing.MinAttribute)attribute;

            if (property.propertyType == SerializedPropertyType.Integer)
            {
                int v = EditorGUI.IntField(position, label, property.intValue);
                property.intValue = (int)Mathf.Max(v, attr.min);
            }
            else if (property.propertyType == SerializedPropertyType.Float)
            {
                float v = EditorGUI.FloatField(position, label, property.floatValue);
                property.floatValue = Mathf.Max(v, attr.min);
            }
            else
            {
                EditorGUI.LabelField(position, label, "Use [Min] with float or int.");
            }
        }
    }
}