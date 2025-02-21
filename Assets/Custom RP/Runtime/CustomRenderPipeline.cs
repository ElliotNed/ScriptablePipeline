using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomRenderPipeline : RenderPipeline
{
    CameraRenderer renderer = new CameraRenderer();
    private ShadowSettings shadowSettings;
    private bool _useGpuInstancing, _useDynamicBatching;

    public CustomRenderPipeline(bool useGpuInstancing, bool useDynamicBatching, bool useSrpBatcher, ShadowSettings shadowSettings)
    {
        _useGpuInstancing = useGpuInstancing;
        _useDynamicBatching = useDynamicBatching;
        GraphicsSettings.useScriptableRenderPipelineBatching = useSrpBatcher;
        GraphicsSettings.lightsUseLinearIntensity = true;
        this.shadowSettings = shadowSettings;
    }

    protected override void Render(ScriptableRenderContext conext, Camera[] cameras)
    {
    }

    protected override void Render(ScriptableRenderContext context, List<Camera> cameras)
    {
        foreach (Camera camera in cameras)
        {
            renderer.Render(context, camera, _useGpuInstancing, _useDynamicBatching, shadowSettings);
        }
    }
}
