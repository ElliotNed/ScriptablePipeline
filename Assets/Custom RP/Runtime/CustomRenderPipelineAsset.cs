using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Rendering/Custom Render Pipeline")]
public class CustomRenderPipelineAsset : RenderPipelineAsset
{
    [SerializeField] private bool
        useGpuInstancing = true,
        useDynamicBatching = true,
        useSrpBatcher = true;

    protected override RenderPipeline CreatePipeline()
    {
        return new CustomRenderPipeline(useGpuInstancing, useDynamicBatching, useSrpBatcher);
    }
}
