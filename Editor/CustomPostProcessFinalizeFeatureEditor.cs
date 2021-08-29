using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEditor.Rendering.Universal;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessFinalizeFeature))]
    public class CustomPostProcessFinalizeFeatureEditor : ScriptableRendererFeatureEditor
    {
        private const string PackageName = "com.ootl.custompostprocess";

        private static string RootPath
        {
#if OOTL_DEV_LOCAL
            get => "Assets/CustomPostProcess";
#else
            get => $"Packages/{PackageName}";
#endif
        }

        // private SerializedProperty _renderPassEvent;
        private SerializedProperty _material;

        private void OnEnable()
        {
            using var settings = serializedObject.FindProperty("settings");

            // _renderPassEvent = settings.FindPropertyRelative("renderPassEvent");
            _material = settings.FindPropertyRelative("material");

            if (_material.objectReferenceValue as Material == null)
            {
                _material.objectReferenceValue =
                    AssetDatabase.LoadAssetAtPath<Material>(
                        $"{RootPath}/Runtime/Materials/CustomPostProcessFinalize.mat");

                serializedObject.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            // _renderPassEvent.intValue = (int) (RenderPassEvent) EditorGUILayout.EnumPopup("RenderPass Event",
            //     (RenderPassEvent) _renderPassEvent.intValue);

            _material.objectReferenceValue =
                EditorGUILayout.ObjectField("Material", _material.objectReferenceValue, typeof(Material), false);

            if (serializedObject.hasModifiedProperties)
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}