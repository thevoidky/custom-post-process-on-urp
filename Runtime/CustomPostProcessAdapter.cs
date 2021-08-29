using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPostProcess.Runtime
{
    public class CustomPostProcessAdapter : MonoBehaviour
    {
        [SerializeField]
        private CustomPostProcessSource target;

        [SerializeField]
        private List<string> features;

        [SerializeField]
        private int featureMask;

        [SerializeField, Range(0f, 5f)]
        private float durationToOn = 0.5f;

        [SerializeField, Range(0f, 5f)]
        private float durationToOff = 0.5f;

        [SerializeField, Range(0f, 1f)]
        private float valueAtOn = 1f;

        [SerializeField, Range(0f, 1f)]
        private float valueAtOff = 0f;

        public UnityEvent onActivationComplete;
        public UnityEvent onDeactivationComplete;

        private CancellationTokenSource _cancellationTokenSource;

        public void Activate(bool isOn)
        {
            Activate(isOn ? valueAtOn : valueAtOff, isOn ? durationToOn : durationToOff);
        }

        public void Activate(float targetValue, float duration)
        {
            if (featureMask == 0)
            {
                Debug.LogWarning($"{GetType()}: featureMask is not set");
                return;
            }

            if (target == null)
            {
                Debug.LogWarning($"{GetType()}: Target CustomPostProcessing is not set");
                return;
            }

            var startValues = new Dictionary<int, float>();
            var endValue = Mathf.Clamp01(targetValue);

            var tempFeatureMask = featureMask;
            var feature = 0;
            while (tempFeatureMask > 0)
            {
                if ((tempFeatureMask & 1) != 0)
                    startValues.Add(feature, target.GetValue(feature));

                tempFeatureMask >>= 1;
                ++feature;
            }

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource ??= new CancellationTokenSource();
            foreach (var pair in startValues)
            {
                Tween(pair.Value, endValue, duration,
                        value => target.SetValue(pair.Key, value),
                        () =>
                        {
                            _cancellationTokenSource?.Dispose();
                            _cancellationTokenSource = null;
                        },
                        () =>
                        {
                            _cancellationTokenSource?.Dispose();
                            _cancellationTokenSource = null;

                            if (Mathf.Approximately(0f, targetValue))
                            {
                                onDeactivationComplete?.Invoke();
                            }
                            else
                            {
                                onActivationComplete?.Invoke();
                            }
                        },
                        _cancellationTokenSource.Token)
                    .Forget();
            }
        }

        private void Awake()
        {
            if (!target)
            {
                return;
            }
            
            // _featureMask = features.Aggregate(0, (_, feature) =>
            // {
            //     ScriptableRendererDat
            //     target.PostProcessEnabled
            // })
        }

        private static async UniTask Tween(float from, float to, float duration, Action<float> onUpdate,
            Action onCancel, Action onComplete, CancellationToken cancellationToken)
        {
            var value = from;
            var addPerSec = (to - from) / duration;
            var min = Mathf.Min(from, to);
            var max = Mathf.Max(from, to);
            var time = duration;

            while (0 < time)
            {
                await UniTask.WaitForEndOfFrame();

                if (cancellationToken.IsCancellationRequested)
                {
                    onCancel?.Invoke();
                    return;
                }

                value = Mathf.Clamp(value + addPerSec * Time.deltaTime, min, max);
                time -= Time.deltaTime;

                onUpdate?.Invoke(value);
            }

            onUpdate?.Invoke(to);
            onComplete?.Invoke();
        }
    }
}