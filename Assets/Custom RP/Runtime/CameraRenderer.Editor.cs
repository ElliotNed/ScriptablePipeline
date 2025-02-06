using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

public partial class CameraRenderer
{
    partial void DrawUnspportedShaders();
    partial void DrawGizmos();
    partial void PrepareForSceneWindow();
    partial void PrepareBuffer();

#if UNITY_EDITOR
    static ShaderTagId[] unsupportedShaderTagIds = {
        new ShaderTagId("Always"),
        new ShaderTagId("ForwardBase"),
        new ShaderTagId("PrepassBase"),
        new ShaderTagId("Vertex"),
        new ShaderTagId("VertexLMRGBM"),
        new ShaderTagId("VertexLM")
    };
    private Material errorMat;

    string sampleName;
    
    partial void DrawUnspportedShaders()
    {
        if (errorMat == null)
            errorMat = new Material(Shader.Find("Hidden/InternalErrorShader"));

        var drawingSettings = new DrawingSettings(unsupportedShaderTagIds[0], new SortingSettings(camera)) { overrideMaterial = errorMat};
        var filteringSettings = FilteringSettings.defaultValue;
        
        for (int i = 1; i < unsupportedShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, unsupportedShaderTagIds[i]);
        }
        context.DrawRenderers(cullingResults, ref drawingSettings, ref filteringSettings);
    }

    partial void DrawGizmos()
    {
        if(Handles.ShouldRenderGizmos())
        {
            context.DrawGizmos(camera, GizmoSubset.PreImageEffects);
            context.DrawGizmos(camera, GizmoSubset.PostImageEffects);
        }
    }

    partial void PrepareForSceneWindow()
    {
        if(camera.cameraType == CameraType.SceneView)
            ScriptableRenderContext.EmitWorldGeometryForSceneView(camera);
    }

    partial void PrepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        buffer.name = sampleName = camera.name;
        Profiler.EndSample();
    }
#else
    const string sampleName = bufferName;
#endif
}
