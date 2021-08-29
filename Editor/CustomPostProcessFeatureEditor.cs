using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessFeature))]
    public class CustomPostProcessFeatureEditor : ScriptableRendererFeatureEditor
    {
        private SerializedProperty _material;

        private void OnEnable()
        {
            using var settings = serializedObject.FindProperty("settings");
            _material = settings.FindPropertyRelative("material");
        }

        public override void OnInspectorGUI()
        {
            _material.objectReferenceValue =
                EditorGUILayout.ObjectField("Material", _material.objectReferenceValue, typeof(Material), false);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}