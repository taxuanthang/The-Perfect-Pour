using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlurRenderFeature : ScriptableRendererFeature
{
    class BlurPass : ScriptableRenderPass
    {
        Material material;
        RTHandle source;
        RTHandle tempTex;
        float blurSize;

        public BlurPass(Material mat, float blur)
        {
            material = mat;
            blurSize = blur;
        }

        public void Setup(RTHandle src)
        {
            source = src;
        }

        // ✅ Unity 6: setup RT ở đây
        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.depthBufferBits = 0;

            tempTex = RTHandles.Alloc(
                desc,
                name: "_TempBlurTex"
            );
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null) return;

            CommandBuffer cmd = CommandBufferPool.Get("URP Blur Pass");

            material.SetFloat("_BlurSize", blurSize);

            // Blur: source -> temp
            Blit(cmd, source, tempTex, material);

            // Copy back: temp -> source
            Blit(cmd, tempTex, source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // ✅ Unity 6: cleanup đúng chỗ
        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (tempTex != null)
            {
                RTHandles.Release(tempTex);
                tempTex = null;
            }
        }
    }

    [Header("Blur Settings")]
    public Shader blurShader;
    public float blurSize = 1f;

    BlurPass blurPass;

    public override void Create()
    {
        if (blurShader == null)
        {
            Debug.LogError("Blur shader missing");
            return;
        }

        Material mat = new Material(blurShader);
        blurPass = new BlurPass(mat, blurSize);

        // Sau khi render xong
        blurPass.renderPassEvent = RenderPassEvent.AfterRendering;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (blurPass == null) return;

        // ✅ Unity 6 API
        blurPass.Setup(renderer.cameraColorTargetHandle);
        renderer.EnqueuePass(blurPass);
    }
}
