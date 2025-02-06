using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    Camera camera;
    ScriptableRenderContext context;

    const string bufferName = "Render Camera";
    CommandBuffer buffer = new CommandBuffer { name = bufferName };

    CullingResults cullingResults;

    static ShaderTagId supportedShaderTagId = new ShaderTagId("SRPDefaultUnlit"); //we only support unlit shaders

    public void Render(ScriptableRenderContext context, Camera camera)
    {
        this.context = context;
        this.camera = camera;

        PrepareForSceneWindow();

        if (!Cull())
            return;

        Setup();
        DrawVisibleGeometry();
        DrawUnspportedShaders();
        DrawGizmos();
        Submit();
    }

    private void DrawVisibleGeometry()
    {
        //render opaque
        var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
        var sortingSettings = new SortingSettings(camera) { criteria = SortingCriteria.CommonOpaque };
        var drawingSettings = new DrawingSettings(supportedShaderTagId, sortingSettings);
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
        buffer.ClearRenderTarget(true, true, Color.clear);
        buffer.BeginSample(bufferName);
        ExecuteBuffer();
    }

    private void Submit()
    {
        buffer.EndSample(bufferName);
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
