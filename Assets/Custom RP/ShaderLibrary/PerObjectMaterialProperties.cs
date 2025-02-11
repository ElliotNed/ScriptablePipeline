using System;
using UnityEngine;

namespace Custom_RP.ShaderLibrary
{
    public class PerObjectMaterialProperties : MonoBehaviour
    {
        private static MaterialPropertyBlock _block;
        private static int _baseColorId = Shader.PropertyToID("_BaseColor");
        private static int _cutoffId = Shader.PropertyToID("_Cutoff");
        
        [SerializeField]
        private Color baseColor = Color.white;
        [SerializeField, Range(0f, 1f)]
        private float cutoff = 0.5f;
        
        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, baseColor);
            _block.SetFloat(_cutoffId, cutoff);
            GetComponent<Renderer>().SetPropertyBlock(_block);
        }
    }
}