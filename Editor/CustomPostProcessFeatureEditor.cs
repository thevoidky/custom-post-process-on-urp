using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessFeature))]
    public class CustomPostProcessFeatureEditor : ScriptableRendererFeatureEditor
    {
        private SerializedProperty _feature;
        private SerializedProperty _material;

        private void OnEnable()
        {
            using var settings = serializedObject.FindProperty("settings");
            _feature = settings.FindPropertyRelative("feature");
            _material = settings.FindPropertyRelative("material");
        }

        public override void OnInspectorGUI()
        {
            _feature.intValue = EditorGUILayout.IntSlider("Feature", _feature.intValue, 0, 31);

            _material.objectReferenceValue =
                EditorGUILayout.ObjectField("Material", _material.objectReferenceValue, typeof(Material), false);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}