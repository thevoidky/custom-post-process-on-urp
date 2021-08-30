using System.Collections.Generic;
using CustomPostProcess.Runtime;
using UnityEditor;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessSource), true), CanEditMultipleObjects]
    public class CustomPostProcessSourceEditor : UnityEditor.Editor
    {
        //  TODO: ScriptableObject화
        // private string[] _names;

        // private SerializedProperty _featureMask;
        private SerializedProperty _globalValueFactor;

        private void OnEnable()
        {
            // _featureMask = serializedObject.FindProperty("featureMask");
            _globalValueFactor = serializedObject.FindProperty("globalValueFactor");

            // var names = new List<string>();
            // for (var i = 0; i < 32; ++i)
            //     names.Add(i.ToString());

            // _names = names.ToArray();
        }

        public override void OnInspectorGUI()
        {
            // _featureMask.intValue = EditorGUILayout.MaskField("Applied Features", _featureMask.intValue, _names);
            _globalValueFactor.floatValue =
                EditorGUILayout.Slider("Global Value Factor", _globalValueFactor.floatValue, 0f, 1f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}