using System.Collections.Generic;
using UnityEngine;
using System;

public class NeuralNetwork : IComparable<NeuralNetwork>
{

    private int[] layers;
    private float[][] neurons; // Jagged array
    // [LAYER][NODE]
    // bv [[1,2,3,4] INPUT, [1,2,3,4] HIDDEN, [1,2,3] HIDDEN, [1,2] OUTPUT]]

    private float[][] biases;
    // [LAYER][NODE]
    // bv [LAYER - 1 [NEURON - 1,2,3,4]]

    private float[][][] weights; // 3 dimensional jagged array
    // [LAYER][NODE][WEIGHT/PREVNEURON]
    // bv [[LAYER 1 [NEURON 1 - 1,2,3,4] [NEURON 2 - 1,2,3,4] [NEURON 3 - 1,2,3,4]]

    private int[] activations;

    private float fitness;

    public NeuralNetwork(int[] layers)
    {
        // Generate layers array
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }
        InitNeurons();
        InitBiases();
        InitWeights();
    }

    // Generate neuron array
    // Go through all layers and add neuron layers
    private void InitNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray(); // Convert list back to array
    }

    // Generate weights array
    private void InitWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();

        for (int i = 1; i < layers.Length; i++)
        {
            List<float[]> layerWeightsList = new List<float[]>();
            int neuronsInPreviousLayer = layers[i - 1]; // Get amount of nodes in prev layer for the connections

            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[neuronsInPreviousLayer];
                for (int k = 0; k < neuronsInPreviousLayer; k++)
                {
                    // Set random float to connecting neuron weights
                    neuronWeights[k] = UnityEngine.Random.Range(-0.5f, 0.5f);
                }

                layerWeightsList.Add(neuronWeights);
            }

            weightsList.Add(layerWeightsList.ToArray());
        }

        weights = weightsList.ToArray(); // Convert weightlist back to 3d jagged array
    }

    // Generate biases
    private void InitBiases()
    {
        List<float[]> biasList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            float[] bias = new float[layers[i]];
            for (int j = 0; j < layers[i]; j++)
            {
                bias[j] = UnityEngine.Random.Range(-0.5f, 0.5f);
            }
            biasList.Add(bias);
        }
        biases = biasList.ToArray();
    }

    // Activation function
    public float activate(float value)
    {
        return (float)Math.Tanh(value);
    }

    // Feed forward
    public float[] FeedForward(float[] inputs)
    {

        // Set input data to first layer
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Iterate over every layer that has a weighted connection (skipping inputs)
        for (int i = 1; i < layers.Length; i++)
        {
            int layer = i - 1;

            // Iterate over every neuron in the layer
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;

                // Iterate over every neuron in the previous layer
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k]; // Get value from previous neuron multiplied by it's weight
                }

                neurons[i][j] = activate(value + biases[i][j]);
            }
        }

        return neurons[neurons.Length - 1];
    }

    public void SetFitness(float fIn)
    {
        fitness = fIn;
    }

    public void SetInput(int i, float value)
    {
        neurons[0][i] = value;
    }

    public float GetInput(int i)
    {
        return neurons[0][i];
    }

    //Comparing performance between networks
    public int CompareTo(NeuralNetwork other)
    {
        if (other == null)
            return 1;
        if (fitness > other.fitness)
            return 1;
        else if (fitness < other.fitness)
            return -1;
        else
            return 0;
    }

    // Deep copy of neural network, ensures serialization
    public NeuralNetwork copy(NeuralNetwork nn)
    {
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                nn.biases[i][j] = biases[i][j];
            }
        }
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    nn.weights[i][j][k] = weights[i][j][k];
                }
            }
        }
        return nn;
    }

    // Simple mutation to biases and weights
    public void Mutate(int chance, float val)
    {
        // Loop through each layer and neuron
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = (UnityEngine.Random.Range(0f, chance) <= 5) ? biases[i][j] += UnityEngine.Random.Range(-val, val) : biases[i][j];
            }
        }

        // Loop through each layer, neuron and weight
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = (UnityEngine.Random.Range(0f, chance) <= 5) ? weights[i][j][k] += UnityEngine.Random.Range(-val, val) : weights[i][j][k];

                }
            }
        }
    }

}
