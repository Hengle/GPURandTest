using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;


public class RandTestSimple : RandTest<RandTestSimple.RandData>
{
    #region type define
    public struct RandData
    {
        public uint seed;
        public float current;
    }
    #endregion

    protected override RandData GetFirstData()
    {
        return new RandData() { seed = (uint)(Random.value * uint.MaxValue) };
    }
}

public abstract class RandTest<DataType> : MonoBehaviour
{

    public ComputeShader _randCS;
    ComputeBuffer _randData;
    ComputeBuffer _appearCounts;

    public int width = 512;
    public int height = 128;
    public RenderTexture _texture;

    public MeshRenderer _targetRenderer;

    int trails;

    protected abstract DataType GetFirstData();

    void Start()
    {
        _randData = new ComputeBuffer(1, Marshal.SizeOf(typeof(DataType)));
        var seed = (uint)(Random.value * uint.MaxValue);
        Debug.Log("seed" + seed);
        _randData.SetData(new[] { GetFirstData() });

        _appearCounts = new ComputeBuffer(width, Marshal.SizeOf(typeof(uint)));
        _appearCounts.SetData(new int[width]);

        _texture = new RenderTexture(width, height, 0);
        _texture.enableRandomWrite = true;
        _texture.Create();

        _targetRenderer.material.mainTexture = _texture;
    }

    void Update()
    {
        _texture.DiscardContents();

        var kernel = _randCS.FindKernel("Calc");
        _randCS.SetBuffer(kernel, "_RandData", _randData);
        _randCS.SetBuffer(kernel, "_AppearCounts", _appearCounts);
        _randCS.Dispatch(kernel, 1, 1, 1);

        kernel = _randCS.FindKernel("Draw");
        _randCS.SetBuffer(kernel, "_RandData", _randData);
        _randCS.SetBuffer(kernel, "_AppearCounts", _appearCounts);
        _randCS.SetTexture(kernel, "_OutputTexture", _texture);
        _randCS.Dispatch(kernel, Mathf.CeilToInt(_texture.width / 8), Mathf.CeilToInt(_texture.height / 8), 1);

        trails++;
    }

    public void OnDestroy()
    {
        _randData.Release();
        _appearCounts.Release();
    }

    public void OnGUI()
    {
        GUILayout.Label("Trials: " + trails);
    }
}
