using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace CustomPostProcess.Runtime
{
    [RequireComponent(typeof(Camera))]
    public class CustomPostProcessSource : MonoBehaviour
    {
        public static readonly int AfterPostProcessTexture = Shader.PropertyToID("_AfterPostProcessTexture");
        public static readonly int CameraColorTexture = Shader.PropertyToID("_CameraColorTexture");
        public static readonly int CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");

        public static readonly int TextureNameOdd = Shader.PropertyToID("_CustomPostProcessBufferOdd");
        public static readonly int TextureNameEven = Shader.PropertyToID("_CustomPostProcessBufferEven");
        public static readonly int PropertyValue = Shader.PropertyToID("_Value");
        public static readonly int PropertyColor = Shader.PropertyToID("_Color");

        [SerializeField]
        private int featureMask = -1;

        [SerializeField]
        private float globalValue = 1f;

        private readonly Dictionary<int, float> _valuesPerFeature = new Dictionary<int, float>();
        private readonly Dictionary<int, float> _offsetsPerFeature = new Dictionary<int, float>();

        public int FeatureMask => featureMask;
        public float GlobalValue => globalValue;

        public bool PostProcessEnabled
        {
            get
            {
                var universalAdditionalCameraData = Camera.GetUniversalAdditionalCameraData();
                return null != universalAdditionalCameraData && universalAdditionalCameraData.renderPostProcessing;
            }
        }

        private Camera Camera => GetComponent<Camera>();

        public bool Contains(int feature) => (featureMask & (1 << feature)) != 0;
        public int Add(int mask) => featureMask |= mask;
        public int Remove(int mask) => featureMask &= ~mask;

        public void SetValue(int feature, float value, bool autoToggleState = true)
        {
            value = Mathf.Clamp01(value);
            _valuesPerFeature[feature] = value;

            if (!autoToggleState)
            {
                return;
            }

            if (value > 0f)
            {
                Add(1 << feature);
            }
            else
            {
                Remove(1 << feature);
            }
        }

// #if UNITY_EDITOR
//         public float GetValue(int feature) => _valuesPerFeature.TryGetValue(feature, out var value) ? value : 1f;
// #else
        public float GetValue(int feature) => _valuesPerFeature.TryGetValue(feature, out var value) ? value : 0f;
// #endif

        public void SetValueOffset(int feature, float offset) => _offsetsPerFeature[feature] = offset;

        public float GetValueOffset(int feature) =>
            _offsetsPerFeature.TryGetValue(feature, out var offset) ? offset : 0f;

        public float GetFinalizedValue(int feature) => (GetValue(feature) + GetValueOffset(feature)) * globalValue;
    }
}