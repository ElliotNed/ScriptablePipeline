using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Shadows
{
    private const string bufferName = "Shadows";
    private CommandBuffer buffer = new CommandBuffer() {name = bufferName};
    
    private ScriptableRenderContext _context;
    private ShadowSettings _shadowSettings;
    private CullingResults _cullingResults;

    private const int _maxShadowedDirectionalLightCount = 1;
    private int _shadowedDirectionalLightCount;
    private ShadowedDirectionalLight[] _shadowedDirectionalLights = new ShadowedDirectionalLight[_maxShadowedDirectionalLightCount];
    
    private static int _dirShadowAtlasId = Shader.PropertyToID("_DirectionalShadowAtlas");
    
    private struct ShadowedDirectionalLight
    {
        public int VisibleLightIndex;
    }

    public void Setup(ScriptableRenderContext context, CullingResults cullingResults, ShadowSettings shadowSettings)
    {
        _context = context;
        _shadowSettings = shadowSettings;
        _cullingResults = cullingResults;
        
        _shadowedDirectionalLightCount = 0;
    }

    public void ExecuteBuffer()
    {
        _context.ExecuteCommandBuffer(buffer);
        buffer.Clear();
    }

    public void ReserveDirectionalShadows(Light light, int visibleLightIndex)
    {
        if (_shadowedDirectionalLightCount < _maxShadowedDirectionalLightCount &&
            light.shadows != LightShadows.None &&
            light.shadowStrength > 0f &&
            _cullingResults.GetShadowCasterBounds(visibleLightIndex, out Bounds b))
        {
            _shadowedDirectionalLights[_shadowedDirectionalLightCount++] = new ShadowedDirectionalLight() {VisibleLightIndex = visibleLightIndex};
        }
    }

    public void Render()
    {
        if (_maxShadowedDirectionalLightCount > 0)
            RenderDirectionalShadows();
        else
            buffer.GetTemporaryRT(_dirShadowAtlasId, 1, 1, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
    }

    private void RenderDirectionalShadows()
    {
        int atlasSize = (int)_shadowSettings.directional.atlasSize;
        buffer.GetTemporaryRT(_dirShadowAtlasId, atlasSize, atlasSize, 32, FilterMode.Bilinear, RenderTextureFormat.Shadowmap);
        buffer.SetRenderTarget(_dirShadowAtlasId, RenderBufferLoadAction.DontCare, RenderBufferStoreAction.Store);
        buffer.ClearRenderTarget(true, false, Color.clear);
        ExecuteBuffer();
    }

    public void Cleanup()
    {
        buffer.ReleaseTemporaryRT(_dirShadowAtlasId);
        ExecuteBuffer();
    }
}
