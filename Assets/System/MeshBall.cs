using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshBall : MonoBehaviour
{
    private static int _baseColorId = Shader.PropertyToID("_BaseColor");
    private static MaterialPropertyBlock _block;

    [SerializeField]
    private Mesh mesh = default;
    [SerializeField]
    Material mat = default;

    private Matrix4x4[] _matrices = new Matrix4x4[1023];
    private Vector4[] _baseColors = new Vector4[1023];
    private void Awake()
    {
        for (int i = 0; i < _matrices.Length; i++)
        {
            _matrices[i] = Matrix4x4.TRS(
                transform.position + Random.insideUnitSphere * 10f,
                Quaternion.Euler(Random.value * 360f, Random.value * 360f, Random.value * 360f),
                Vector3.one * Random.Range(.5f, 1.5f));
            
            _baseColors[i] = new Vector4(Random.value, Random.value, Random.value, Random.Range(.1f, 1f));
        }
    }

    private void Update()
    {
        if (_block == null)
        {
            _block = new MaterialPropertyBlock();
            _block.SetVectorArray(_baseColorId, _baseColors);
        }

        Graphics.DrawMeshInstanced(mesh, 0, mat, _matrices, 1023, _block);
    }
}