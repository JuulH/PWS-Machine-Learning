using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ExportNetwork
{
    public int[] layers;
    public float[][] neurons;
    public float[][] biases;
    public float[][][] weights;

    public ExportNetwork(int[] _layers, float[][] _neurons, float[][] _biases, float[][][] _weights)
    {
        this.layers = _layers;
        this.neurons = _neurons;
        this.biases = _biases;
        this.weights = _weights;
    }
}