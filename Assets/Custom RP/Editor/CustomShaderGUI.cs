using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class CustomShaderGUI : ShaderGUI
{
    private MaterialEditor editor;
    private Object[] materials;
    private MaterialProperty[] properties;

    private bool showPresets;
    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        base.OnGUI(materialEditor, properties);
        editor = materialEditor;
        materials = materialEditor.targets;
        this.properties = properties;
        
        EditorGUILayout.Space();
        showPresets = EditorGUILayout.Foldout(showPresets, "Presets");
        if (showPresets)
        {
            OpaquePreset();
            ClipPreset();
            FadePreset();
            TransparentPreset();
        }
    }

    private bool SetProperty(string name, float value)
    {
        var property = FindProperty(name, properties, false);
        if (property != null)
        {
            property.floatValue = value;
            return true;
        };
        return false;
    }

    private void SetProperty(string name, string keyword, bool value)
    {
        if(SetProperty(name, value? 1:0))
            SetKeyword(keyword, value);
    }

    private void SetKeyword(string name, bool enabled)
    {
        if (enabled)
        {
            foreach (Material mat in materials)
                mat.EnableKeyword(name);
        }
        else
        {
            foreach (Material mat in materials)
                mat.DisableKeyword(name);
        }
    }
    
    bool PresetButton (string name)
    {
        if (GUILayout.Button(name))
        {
            editor.RegisterPropertyChangeUndo(name);
            return true;
        }
        return false;
    }
    
    void OpaquePreset ()
    {
        if (PresetButton("Opaque"))
        {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.Geometry;
        }
    }
    
    void ClipPreset ()
    {
        if (PresetButton("Clip"))
        {
            Clipping = true;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.Zero;
            ZWrite = true;
            RenderQueue = RenderQueue.AlphaTest;
        }
    }
    
    void FadePreset ()
    {
        if (PresetButton("Fade"))
        {
            Clipping = false;
            PremultiplyAlpha = false;
            SrcBlend = BlendMode.SrcAlpha;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
    
    void TransparentPreset ()
    {
        if (HasPremultiplyAlpha && PresetButton("Transparent"))
        {
            Clipping = false;
            PremultiplyAlpha = true;
            SrcBlend = BlendMode.One;
            DstBlend = BlendMode.OneMinusSrcAlpha;
            ZWrite = false;
            RenderQueue = RenderQueue.Transparent;
        }
    }
    
    bool HasProperty (string name) =>
        FindProperty(name, properties, false) != null;
    
    bool HasPremultiplyAlpha => HasProperty ("_PremultAlpha");
    
    bool Clipping
    {
        set => SetProperty("_Clipping", "_CLIPPING", value);
    }

    bool PremultiplyAlpha
    {
        set => SetProperty("_PremultAlpha", "_PREMULTIPLY_ALPHA", value);
    }

    BlendMode SrcBlend
    {
        set => SetProperty("_SrcBlend", (float)value);
    }

    BlendMode DstBlend
    {
        set => SetProperty("_DstBlend", (float)value);
    }

    bool ZWrite
    {
        set => SetProperty("_ZWrite", value ? 1f : 0f);
    }

    RenderQueue RenderQueue
    {
        set
        {
            foreach (Material mat in materials)
            {
                mat.renderQueue = (int)value;
            }
        }
    }
}
