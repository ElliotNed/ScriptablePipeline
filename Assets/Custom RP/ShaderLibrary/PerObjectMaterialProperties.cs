using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

namespace Custom_RP.ShaderLibrary
{
    public class PerObjectMaterialProperties : MonoBehaviour
    {
        private static MaterialPropertyBlock _block;

        private static readonly int
            BaseColorId = Shader.PropertyToID("_BaseColor"),
            CutoffId = Shader.PropertyToID("_Cutoff"),
            MetallicId = Shader.PropertyToID("_Metallic"),
            SmoothnessId = Shader.PropertyToID("_Smoothness");
        
        [SerializeField]
        private Color baseColor = Color.white;

        [SerializeField, Range(0f, 1f)] private float
            cutoff = 0.5f,
            metallic,
            smoothness = 0.5f;
        
        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _block ??= new MaterialPropertyBlock();
            _block.SetColor(BaseColorId, baseColor);
            _block.SetFloat(CutoffId, cutoff);
            _block.SetFloat(MetallicId, metallic);
            _block.SetFloat(SmoothnessId, smoothness);
            GetComponent<Renderer>().SetPropertyBlock(_block);
        }
    }
}