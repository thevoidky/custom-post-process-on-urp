using System.Collections.Generic;
using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessAdapter), true)]
    //[CanEditMultipleObjects]
    public class CustomPostProcessAdapterEditor : UnityEditor.Editor
    {
        //  TODO: ScriptableObject화
        private string[] _names;

        private SerializedProperty _customPostProcessing;
        private SerializedProperty _featureMask;
        private SerializedProperty _durationToOn, _durationToOff;
        private SerializedProperty _valueAtOn, _valueAtOff;
        private SerializedProperty _onActivationComplete, _onDeactivationComplete;

        private bool _isOn;

        private void OnEnable()
        {
            _customPostProcessing = serializedObject.FindProperty("target");
            _featureMask = serializedObject.FindProperty("featureMask");
            _durationToOn = serializedObject.FindProperty("durationToOn");
            _durationToOff = serializedObject.FindProperty("durationToOff");
            _valueAtOn = serializedObject.FindProperty("valueAtOn");
            _valueAtOff = serializedObject.FindProperty("valueAtOff");
            _onActivationComplete = serializedObject.FindProperty("onActivationComplete");
            _onDeactivationComplete = serializedObject.FindProperty("onDeactivationComplete");

            var names = new List<string>();
            for (var i = 0; i < 32; ++i)
            {
                names.Add(i.ToString());
            }

            _names = names.ToArray();
        }

        public override void OnInspectorGUI()
        {
            _customPostProcessing.objectReferenceValue = EditorGUILayout.ObjectField("Source",
                _customPostProcessing.objectReferenceValue,
                typeof(CustomPostProcessSource), true);

            if (null == _customPostProcessing.objectReferenceValue)
            {
                return;
            }

            _featureMask.intValue = EditorGUILayout.MaskField("Applied Features", _featureMask.intValue, _names);
            _durationToOn.floatValue = EditorGUILayout.Slider("Duration to on", _durationToOn.floatValue, 0f, 5f);
            _durationToOff.floatValue = EditorGUILayout.Slider("Duration to off", _durationToOff.floatValue, 0f, 5f);

            _valueAtOn.floatValue = EditorGUILayout.Slider("Value to on", _valueAtOn.floatValue, 0f, 1f);
            _valueAtOff.floatValue = EditorGUILayout.Slider("Value to off", _valueAtOff.floatValue, 0f, 1f);

            _valueAtOff.floatValue = Mathf.Min(_valueAtOn.floatValue, _valueAtOff.floatValue);
            _valueAtOn.floatValue = Mathf.Max(_valueAtOn.floatValue, _valueAtOff.floatValue);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(_onActivationComplete);
            EditorGUILayout.PropertyField(_onDeactivationComplete);

            serializedObject.ApplyModifiedProperties();

            var cpp = _customPostProcessing.objectReferenceValue as CustomPostProcessSource;
            if (cpp)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button($"Toggle to {(_isOn ? "off" : "on")}", GUILayout.Height(32f)))
                {
                    var adapter = target as CustomPostProcessAdapter;
                    if (adapter)
                    {
                        adapter.Activate(_isOn = !_isOn);
                    }
                }
            }
        }
    }
}