using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualizeNetwork : MonoBehaviour
{

    private int[] layers;
    private GameObject[][] neurons;
    private LineRenderer[][][] weights;

    [SerializeField] private GameObject neuronPrefab;
    [SerializeField] private GameObject linePrefab;
    [SerializeField] private float verticalGap;
    [SerializeField] private float horizontalGap;
    private Vector2 startPoint;

    private void CreateArrays()
    {
        // Setup neuron array
        List<GameObject[]> neuronsList = new List<GameObject[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new GameObject[layers[i]]);
        }
        neurons = neuronsList.ToArray(); // Convert list back to array

        // Setup weights array
        List<LineRenderer[][]> weightsList = new List<LineRenderer[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            List<LineRenderer[]> layerWeightsList = new List<LineRenderer[]>();
            int neuronsInPreviousLayer = layers[i - 1]; // Get amount of nodes in prev layer for the connections

            for (int j = 0; j < neurons[i].Length; j++)
            {
                LineRenderer[] neuronWeights = new LineRenderer[neuronsInPreviousLayer];

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray(); // Convert weightlist back to 3d jagged array
    }

    public void CreateNetwork(int[] _layers)
    {
        layers = _layers;
        CreateArrays();

        // Create neurons
        float minX = -((layers.Length - 1 ) * horizontalGap) / 2 + transform.position.x;

        for (int i = 0; i < layers.Length; i++)
        {
            float minY = ((layers[i] - 1 ) * verticalGap) / 2 + transform.position.y;
            startPoint = new Vector2(minX, minY);
            for (int j = 0; j < layers[i]; j++)
            {
                GameObject newNeuron = Instantiate(neuronPrefab, startPoint + new Vector2(i * horizontalGap, j * -verticalGap), Quaternion.identity, transform);
                neurons[i][j] = newNeuron;
            }
        }

        // Create connections
        for (int i = 1; i < neurons.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                for (int k = 0; k < neurons[i-1].Length; k++)
                {
                    GameObject newWeight = Instantiate(linePrefab, neurons[i][j].transform.position, Quaternion.identity, transform);
                    LineRenderer line = newWeight.GetComponent<LineRenderer>();
                    line.SetPosition(0, neurons[i - 1][k].transform.position);
                    line.SetPosition(1, neurons[i][j].transform.position);
                    weights[i-1][j][k] = line;
                }
            }
        }
    }

    public void UpdateNetwork(float[][] _neurons, float[][][] _weights)
    {
        for (int i = 0; i < _neurons.Length; i++)
        {
            for (int j = 0; j < _neurons[i].Length; j++)
            {
                float value = _neurons[i][j];
                float alpha = (value + 1) / 2;

                SpriteRenderer sprite = neurons[i][j].GetComponent<SpriteRenderer>();
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, alpha);
            }
        }

        for (int i = 0; i < _weights.Length; i++)
        {
            for (int j = 0; j < _weights[i].Length; j++)
            {
                for (int k = 0; k < _weights[i][j].Length; k++)
                {
                    float weight = _weights[i][j][k];
                    float thickness = ((weight + 1) / 2) * 0.15f;

                    LineRenderer line = weights[i][j][k];
                    line.startWidth = thickness;
                    line.endWidth = thickness;

                    float alpha = ((weight + 1) / 2);
                    Color color = line.startColor;
                    Color newColor = new Color(color.r, color.g, color.b, alpha);
                    line.startColor = newColor;
                    line.endColor = newColor;
                }
            }
        }
    }
}
