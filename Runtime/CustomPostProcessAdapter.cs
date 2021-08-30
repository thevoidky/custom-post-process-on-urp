using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

namespace CustomPostProcess.Runtime
{
    [Serializable]
    public class CustomPostProcessAdapter
    {
        [SerializeField]
        private CustomPostProcessSource target;

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

        public CustomPostProcessSource Target
        {
            get => target;
            set => target = value;
        }

        public int FeatureMask
        {
            get => featureMask;
            set => featureMask = value;
        }

        public float DurationToOn
        {
            get => durationToOn;
            set => durationToOn = value;
        }

        public float DurationToOff
        {
            get => durationToOff;
            set => durationToOff = value;
        }

        public float ValueAtOn
        {
            get => valueAtOn;
            set => valueAtOn = value;
        }

        public float ValueAtOff
        {
            get => valueAtOff;
            set => valueAtOff = value;
        }

        private CancellationTokenSource _cancellationTokenSource;

        public void Activate(bool isOn)
        {
            Activate(isOn ? valueAtOn : valueAtOff, isOn ? durationToOn : durationToOff);
        }

        private void Activate(float targetValue, float duration)
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