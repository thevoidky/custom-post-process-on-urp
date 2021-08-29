using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessInitializeFeature))]
    public class CustomPostProcessInitializeFeatureEditor : ScriptableRendererFeatureEditor
    {
        // private SerializedProperty _renderPassEvent;
        private SerializedProperty _filterMode;
        private SerializedProperty _downsample;

        private void OnEnable()
        {
            using var settings = serializedObject.FindProperty("settings");

            // _renderPassEvent = settings.FindPropertyRelative("renderPassEvent");
            _filterMode = settings.FindPropertyRelative("filterMode");
            _downsample = settings.FindPropertyRelative("downsample");
        }

        public override void OnInspectorGUI()
        {
            // _renderPassEvent.intValue = (int) (RenderPassEvent) EditorGUILayout.EnumPopup("RenderPass Event",
            //     (RenderPassEvent) _renderPassEvent.intValue);

            _filterMode.enumValueIndex = (int) (FilterMode) EditorGUILayout.EnumPopup("Filter Mode",
                (FilterMode) _filterMode.enumValueIndex);

            _downsample.intValue = EditorGUILayout.IntSlider("Downsampling Level", _downsample.intValue, 0, 3);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}