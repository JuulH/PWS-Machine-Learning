using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{

    private bool training;
    [SerializeField] private int populationSize;
    [SerializeField] private int generation;
    [SerializeField] private int alive;

    [SerializeField] private float highScore;
    [SerializeField] private float highScoreCurrentGen;
    [SerializeField] private float avgScoreCurrentGen;
    [SerializeField] private float lowScoreCurrentGen;

    // Neural network config
    private int[] layers = new int[] { 6, 8, 8, 1 };
    private List<NeuralNetwork> networks;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(training)
        {
            if(alive <= 0)
            {
                // Sort networks by fitness ascending
                networks.Sort();

                
            }
        }
    }

    // Initialize network with random weights and biases
    private void InitNetwork()
    {
        networks = new List<NeuralNetwork>();

        for(int i = 0; i < populationSize; i++)
        {
            NeuralNetwork net = new NeuralNetwork(layers);
            networks.Add(net);
        }
    }
}
