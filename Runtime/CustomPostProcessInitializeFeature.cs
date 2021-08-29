using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomPostProcess.Runtime
{
    internal struct CustomPostProcessRenderTargetProperty
    {
        public RenderTargetIdentifier renderTargetIdentifier;
        public readonly int id;
        public readonly int width, height;

        public CustomPostProcessRenderTargetProperty(CommandBuffer cmd, int width, int height, int id,
            FilterMode filterMode)
        {
            this.id = id;
            cmd.GetTemporaryRT(id, width, height, 0, filterMode, RenderTextureFormat.ARGB32);
            renderTargetIdentifier = new RenderTargetIdentifier(id);
            this.width = width;
            this.height = height;
        }
    }

    internal class CustomPostProcessInitializeRenderPass : ScriptableRenderPass
    {
        private readonly int _downsample = 0;
        private readonly FilterMode _filterMode;

        private readonly CustomPostProcessRenderTargetProperty[] _properties =
            new CustomPostProcessRenderTargetProperty[2];

        public CustomPostProcessInitializeRenderPass(RenderPassEvent renderPassEvent, FilterMode filterMode,
            int downsample)
        {
            _filterMode = filterMode;
            _downsample = downsample;
            this.renderPassEvent = renderPassEvent;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            var width = cameraTextureDescriptor.width >> _downsample;
            var height = cameraTextureDescriptor.height >> _downsample;

            _properties[0] = new CustomPostProcessRenderTargetProperty(cmd, width, height,
                CustomPostProcessSource.TextureNameOdd, _filterMode);
            _properties[1] = new CustomPostProcessRenderTargetProperty(cmd, width, height,
                CustomPostProcessSource.TextureNameEven, _filterMode);

            ConfigureTarget(_properties[0].renderTargetIdentifier);
            ConfigureTarget(_properties[1].renderTargetIdentifier);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // do nothing
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            foreach (var property in _properties)
            {
                cmd.ReleaseTemporaryRT(property.id);
            }

            CustomPostProcessRenderPass.flipIndex = 0;
        }
    }

    [Serializable]
    public class InitializeFeatureSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
        public FilterMode filterMode = FilterMode.Bilinear;

        [Range(0, 4)]
        public int downsample = 0;
    }

    public class CustomPostProcessInitializeFeature : ScriptableRendererFeature
    {
        // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
        [SerializeField]
        private InitializeFeatureSettings settings = new InitializeFeatureSettings();

        private CustomPostProcessInitializeRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new CustomPostProcessInitializeRenderPass(settings.renderPassEvent,
                settings.filterMode, settings.downsample);
        }

        // called every frame once per camera
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            // 카메라 없이 작동하지 않음
            var camera = renderingData.cameraData.camera;
            if (camera == null)
            {
                return;
            }

            // Custom Post Processing이 없는 카메라에서 패스를 실행하지 않음
            if (!camera.TryGetComponent<CustomPostProcessSource>(out _))
            {
                return;
            }

            renderer.EnqueuePass(_renderPass);
        }
    }
}