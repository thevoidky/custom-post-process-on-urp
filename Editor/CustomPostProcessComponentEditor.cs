using System.Collections.Generic;
using CustomPostProcess.Runtime;
using UnityEditor;
using UnityEngine;

namespace CustomPostProcess.Editor
{
    [CustomEditor(typeof(CustomPostProcessComponent), true)]
    //[CanEditMultipleObjects]
    public class CustomPostProcessComponentEditor : UnityEditor.Editor
    {
        //  TODO: ScriptableObject화
        private string[] _names;

        private SerializedProperty _target;
        private SerializedProperty _featureMask;
        private SerializedProperty _durationToOn, _durationToOff;
        private SerializedProperty _valueAtOn, _valueAtOff;
        private SerializedProperty _onActivationComplete, _onDeactivationComplete;

        private bool _isOn;

        private void OnEnable()
        {
            var adapter = serializedObject.FindProperty("adapter");

            _target = adapter.FindPropertyRelative("target");
            _featureMask = adapter.FindPropertyRelative("featureMask");
            _durationToOn = adapter.FindPropertyRelative("durationToOn");
            _durationToOff = adapter.FindPropertyRelative("durationToOff");
            _valueAtOn = adapter.FindPropertyRelative("valueAtOn");
            _valueAtOff = adapter.FindPropertyRelative("valueAtOff");
            _onActivationComplete = adapter.FindPropertyRelative("onActivationComplete");
            _onDeactivationComplete = adapter.FindPropertyRelative("onDeactivationComplete");

            var names = new List<string>();
            for (var i = 0; i < 32; ++i)
            {
                names.Add(i.ToString());
            }

            _names = names.ToArray();
        }

        public override void OnInspectorGUI()
        {
            _target.objectReferenceValue = EditorGUILayout.ObjectField("Source",
                _target.objectReferenceValue,
                typeof(CustomPostProcessSource), true);

            if (null == _target.objectReferenceValue)
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

            var cpp = _target.objectReferenceValue as CustomPostProcessSource;
            if (cpp)
            {
                EditorGUILayout.Space();
                if (GUILayout.Button($"Toggle to {(_isOn ? "off" : "on")}", GUILayout.Height(32f)))
                {
                    var adapter = target as CustomPostProcessComponent;
                    if (adapter)
                    {
                        adapter.Activate(_isOn = !_isOn);
                    }
                }
            }
        }
    }
}