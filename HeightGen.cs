using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System;

public class HeightGen : MonoBehaviour
{
    float seed = 11f;
    public int grize = 16;
    public ComputeShader getter;
    ComputeBuffer buff;
    int handle;
    public QuadTreeChunkController test;

    private void Awake() {
        buff = new ComputeBuffer(256, 12);
    }
    [System.NonSerialized] public Vector3[] h = new Vector3[256];

    public void GPUHeightGetter(Vector3 start, float scale) {       
        handle = getter.FindKernel("HeightAlgorithm");
        getter.SetBuffer(handle, "buff", buff);
        getter.SetFloat(Shader.PropertyToID("x"), start.x);
        getter.SetFloat(Shader.PropertyToID("z"), start.z);
        getter.SetFloat(Shader.PropertyToID("scale"), scale);
        getter.SetFloat(Shader.PropertyToID("seed"), seed);
        getter.Dispatch(handle, 1, 1, 1);
        buff.GetData(h);
    }

    private void OnDestroy() {
        buff.Dispose();
    }

}
