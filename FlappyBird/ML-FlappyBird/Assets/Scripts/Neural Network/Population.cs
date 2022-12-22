using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{

    [SerializeField] private bool training;

    [SerializeField] private int populationSize;
    [SerializeField] private int generation;
    [SerializeField] private int alive;

    [SerializeField] private GameObject agentPrefab;

    [SerializeField] private float highScore;
    [SerializeField] private float highScoreCurrentGen;
    [SerializeField] private float avgScoreCurrentGen;
    [SerializeField] private float lowScoreCurrentGen;

    // Neural network config
    [SerializeField] private int[] layers = new int[] { 6, 8, 8, 1 };
    private List<NeuralNetwork> networks;

    [SerializeField] private PipeSpawner pipe;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private bool trackFittest;

    // Start is called before the first frame update
    void Start()
    {
        InitNetworks();
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

                
            } else
            {
                foreach(NeuralNetwork net in networks)
                {
                    (float, float) pipeInput = pipe.GetPipeInput();
                    net.SetInput(1, pipeInput.Item1);
                    net.SetInput(2, pipeInput.Item2);
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.F))
        {
            cameraFollow.followTarget = !cameraFollow.followTarget;
        }

        //cameraFollow.target = Vector2.zero;
    }

    // Initialize network with random weights and biases
    private void InitNetworks()
    {
        networks = new List<NeuralNetwork>();

        for(int i = 0; i < populationSize; i++)
        {
            NeuralNetwork network = new NeuralNetwork(layers);
            networks.Add(network);

            Agent bird = Instantiate(agentPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Agent>();
            bird.InitAgent(network, i);
        }
    }
}
