using System;
using UnityEngine;

namespace Custom_RP.ShaderLibrary
{
    public class PerObjectMaterialProperties : MonoBehaviour
    {
        [SerializeField]
        private Color baseColor = Color.white;
        
        private static MaterialPropertyBlock _block;
        private static int _baseColorId = Shader.PropertyToID("_BaseColor");

        private void Awake()
        {
            OnValidate();
        }

        private void OnValidate()
        {
            _block ??= new MaterialPropertyBlock();
            _block.SetColor(_baseColorId, baseColor);
            GetComponent<Renderer>().SetPropertyBlock(_block);
        }
    }
}