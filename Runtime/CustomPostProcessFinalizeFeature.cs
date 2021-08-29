using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CustomPostProcess.Runtime
{
    internal class CustomPostProcessFinalizeRenderPass : ScriptableRenderPass
    {
        private readonly string _profilerTag;
        private CustomPostProcessSource _cpp;
        private RenderTargetHandle _cppRenderSourceHandle;
        private RenderTargetHandle _afterPostProcessTextureHandle;
        private RenderTargetIdentifier _cameraColorTarget;
        private readonly Material _material;

        public CustomPostProcessFinalizeRenderPass(string profilerTag,
            CustomPostProcessFinalizeFeature.FeatureSettings settings)
        {
            _profilerTag = profilerTag;
            renderPassEvent = settings.renderPassEvent;
            _material = settings.material;

            _afterPostProcessTextureHandle.Init("_AfterPostProcessTexture");
        }

        public void Setup(RenderTargetIdentifier source, CustomPostProcessSource cpp)
        {
            _cameraColorTarget = source;
            _cpp = cpp;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            //  렌더링되지 않았으면 실행하지 않음
            if (CustomPostProcessRenderPass.flipIndex == 0)
            {
                return;
            }

            // fetch a command buffer to use
            var cmd = CommandBufferPool.Get(_profilerTag);
            cmd.Clear();

            _cppRenderSourceHandle.Init(CustomPostProcessRenderPass.flipIndex % 2 == 1
                ? CustomPostProcessSource.TextureNameOdd
                : CustomPostProcessSource.TextureNameEven);

            _material.SetFloat(CustomPostProcessSource.PropertyValue, _cpp.GlobalValue);
            // _material.SetFloat(CustomPostProcessing.PropertyValue, 1f);

            var dst = renderingData.cameraData.postProcessEnabled
                ? _afterPostProcessTextureHandle.Identifier()
                : _cameraColorTarget;
            cmd.Blit(_cppRenderSourceHandle.Identifier(), dst, _material);

            // don't forget to tell ScriptableRenderContext to actually execute the commands
            context.ExecuteCommandBuffer(cmd);

            // tidy up after ourselves
            cmd.Clear();
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            CustomPostProcessRenderPass.flipIndex = 0;
        }
    }

    public class CustomPostProcessFinalizeFeature : ScriptableRendererFeature
    {
        [Serializable]
        public class FeatureSettings
        {
            public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRendering;
            public Material material;
        }

        // MUST be named "settings" (lowercase) to be shown in the Render Features inspector
        [SerializeField]
        private FeatureSettings settings = new FeatureSettings();

        private CustomPostProcessFinalizeRenderPass _renderPass;

        public override void Create()
        {
            _renderPass = new CustomPostProcessFinalizeRenderPass(name, settings);
        }

        // called every frame once per camera
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            //  카메라 없이 작동하지 않음
            var camera = renderingData.cameraData.camera;
            if (camera == null)
            {
                return;
            }

            //  Custom Post Processing이 없는 카메라에서 패스를 실행하지 않음
            var cpp = camera.GetComponent<CustomPostProcessSource>();
            if (cpp == null)
            {
                return;
            }

            var cameraColorTarget = renderer.cameraColorTarget;
            _renderPass.Setup(cameraColorTarget, cpp);

            renderer.EnqueuePass(_renderPass);
        }
    }
}