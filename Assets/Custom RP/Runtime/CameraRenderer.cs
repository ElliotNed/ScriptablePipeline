using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    Camera camera;
    Lighting lighting = new Lighting();
    ScriptableRenderContext context;

    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    CullingResults cullingResults;

    static ShaderTagId UnlitShaderTagId = new ShaderTagId("SRPDefaultUnlit");
    static ShaderTagId LitPassShaderTagId = new ShaderTagId("CustomLit");

    public void Render(ScriptableRenderContext context, Camera camera, bool useGpuInstancing, bool useDynamicBatching)
    {
        this.context = context;
        this.camera = camera;

        PrepareBuffer();
        PrepareForSceneWindow();

        if (!Cull())
            return;

        Setup();
        lighting.Setup(context, cullingResults);
        DrawVisibleGeometry(useGpuInstancing, useDynamicBatching);
        DrawUnspportedShaders();
        DrawGizmos();
        Submit();
    }

    private void DrawVisibleGeometry(bool useGpuInstancing, bool useDynamicBatching)
    {
        //render opaque
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        var sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
        var drawingSettings = new DrawingSettings(UnlitShaderTagId, sortingSettings)
        {
            enableInstancing = useGpuInstancing,
            enableDynamicBatching = useDynamicBatching
        };
        drawingSettings.SetShaderPassName(1, LitPassShaderTagId); //adding support to lit shader
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);

        context.DrawSkybox(camera);

        //render transparent
        filteringSettings.renderQueueRange = RenderQueueRange.transparent;
        sortingSettings.criteria = SortingCriteria.CommonTransparent;
        drawingSettings.sortingSettings = sortingSettings;
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    private void Setup()
    {
        context.SetupCameraProperties(camera);
        CameraClearFlags flags = camera.clearFlags;
        buffer.ClearRenderTarget(flags <= CameraClearFlags.Depth, flags <= CameraClearFlags.Color, flags == CameraClearFlags.Color? camera.backgroundColor.linear : Color.clear);
        buffer.BeginSample(sampleName);
        ExecuteBuffer();
    }

    private void Submit()
    {
        buffer.EndSample(sampleName);
        ExecuteBuffer();
        context.Submit();
    }

    private void ExecuteBuffer()
    {
        context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    private bool Cull()
    {
        if(camera.TryGetCullingParameters(out ScriptableCullingParameters param))
        {
            cullingResults = context.Cull(ref param);
            return true;
        }
        return false;
    }
}
