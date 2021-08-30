using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomPostProcess.Runtime
{
    internal class CustomPostProcessRenderPass : ScriptableRenderPass
    {
        internal static int flipIndex = 0;

        private readonly string _profilerTag;

        private readonly Material _material;
        private RenderTargetHandle _afterPostProcessTextureHandle;
        private RenderTargetHandle _cameraColorTextureHandle;
        private RenderTargetHandle _cameraDepthTextureHandle;

        // private RenderTargetIdentifier _cameraColorTarget;
        // private RenderTargetIdentifier _cameraDepthTarget;
        private CustomPostProcessSource _cpp;

        private readonly int _feature;
        private readonly Color _color;

        private readonly RenderTargetHandle[] _renderTargetHandles = new RenderTargetHandle[2];

        public CustomPostProcessRenderPass(string profilerTag, RenderPassEvent renderPassEvent, Material material,
            int feature, Color color)
        {
            _profilerTag = profilerTag;
            this.renderPassEvent = renderPassEvent;
            _material = material;
            _feature = feature;
            _color = color;

            _cameraColorTextureHandle.Init(CustomPostProcessSource.CameraColorTexture);
            _cameraDepthTextureHandle.Init(CustomPostProcessSource.CameraDepthTexture);
            _afterPostProcessTextureHandle.Init(CustomPostProcessSource.AfterPostProcessTexture);

            _renderTargetHandles[0].Init(CustomPostProcessSource.TextureNameOdd);
            _renderTargetHandles[1].Init(CustomPostProcessSource.TextureNameEven);
        }

        // public void Setup(RenderTargetIdentifier cameraColorTarget, CustomPostProcess cpp)
        // {
        //     // _cameraColorTarget = cameraColorTarget;
        //     _cpp = cpp;
        // }

        public void Setup(CustomPostProcessSource cpp) => _cpp = cpp;

        // public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        // {
        //     _renderTargetHandles[0].Init(CustomPostProcess.TextureNameOdd);
        //     _renderTargetHandles[1].Init(CustomPostProcess.TextureNameEven);
        // }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            // fetch a command buffer to use
            var cmd = CommandBufferPool.Get(_profilerTag);
            cmd.Clear();

            var flipFlag = flipIndex == 0 ? null : (flipIndex % 2 == 1) as bool?;
            var identifier = renderingData.postProcessingEnabled
                ? _afterPostProcessTextureHandle.Identifier()
                : _cameraColorTextureHandle.Identifier();
            var (src, dst) = flipFlag switch
            {
                null => (identifier, _renderTargetHandles[0].Identifier()),
                true => (_renderTargetHandles[0].Identifier(), _renderTargetHandles[1].Identifier()),
                false => (_renderTargetHandles[1].Identifier(), _renderTargetHandles[0].Identifier())
            };

            _material.SetFloat(CustomPostProcessSource.PropertyValue, _cpp.GetFinalizedValue(_feature));
            _material.SetColor(CustomPostProcessSource.PropertyColor, _color);
            cmd.Blit(src, dst, _material, 0);
            ++flipIndex;

            // don't forget to tell ScriptableRenderContext to actually execute the commands
            context.ExecuteCommandBuffer(cmd);

            // tidy up after ourselves
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        // called after Execute, use it to clean up anything allocated in Configure
        // public override void FrameCleanup(CommandBuffer cmd)
        // {
        // }
    }

    public class CustomPostProcessFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class FeatureSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
            public Material material;

            [Range(0, 31)]
            public int feature = 0;

            public Color color = Color.white;
        }

        // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
        [SerializeField]
        private FeatureSettings settings = new FeatureSettings();

        private CustomPostProcessRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new CustomPostProcessRenderPass(name, settings.renderPassEvent, settings.material,
                settings.feature, settings.color);
        }

        // called every frame once per camera
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //  카메라 없이 작동하지 않음
            var camera = renderingData.cameraData.camera;
            if (camera == null) return;

            //  Custom Post Processing이 없는 카메라에서 패스를 실행하지 않음
            if (!camera.TryGetComponent<CustomPostProcessSource>(out var cpp))
            {
                return;
            }

            //  사용하지 않는 패스를 실행하지 않음
            if (!cpp.Contains(settings.feature)) return;

            // _renderPass.Setup(renderer.cameraColorTarget, cpp);
            _renderPass.Setup(cpp);

            renderer.EnqueuePass(_renderPass);
        }
    }
}