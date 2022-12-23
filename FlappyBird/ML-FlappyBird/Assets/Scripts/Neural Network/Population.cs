using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population : MonoBehaviour
{

    [SerializeField] private bool training;

    [SerializeField] private int populationSize;
    [SerializeField] private int generation;
    [SerializeField] private int alive;

    private GameObject[] agents;

    [SerializeField] private GameObject agentPrefab;

    [SerializeField] private float highScore;
    [SerializeField] private float highScoreCurrentGen;
    [SerializeField] private float avgScoreCurrentGen;
    [SerializeField] private float lowScoreCurrentGen;

    // Neural network config
    [SerializeField] private int[] layers = new int[] { 6, 8, 8, 1 };
    private List<NeuralNetwork> networks;
    private List<GameObject> birds;

    // Mutation
    private float elitistPercentage = 0.1f;
    private float crossoverPercentage = 0.25f;
    private float mutationPercentage = 0.65f;

    [SerializeField] private PipeSpawner pipe;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private bool trackFittest;

    // Start is called before the first frame update
    void Start()
    {
        InitNetworks();
        pipe.SetActive();
    }

    // Update is called once per frame
    void Update()
    {
        agents = GameObject.FindGameObjectsWithTag("Agent");
        alive = agents.Length;

        if(training)
        {
            if(alive <= 0)
            {
                // Sort networks by fitness ascending
                networks.Sort();
                Debug.Log(networks[0].GetFitness());
                pipe.Clear();

                MutateNetworks();
                ResetNetworks();
                pipe.SetActive();

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
        birds = new List<GameObject>();

        for(int i = 0; i < populationSize; i++)
        {
            NeuralNetwork network = new NeuralNetwork(layers);
            network.SetID(i);
            networks.Add(network);

            Agent bird = Instantiate(agentPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Agent>();
            bird.InitAgent(network, i);
            birds.Add(bird.gameObject);
        }
    }

    // Reset networks for new generation
    private void ResetNetworks()
    {
        foreach(GameObject bird in birds)
        {
            bird.SetActive(true);
        }
        foreach(NeuralNetwork network in networks)
        {
            BirdController bird = GameObject.FindGameObjectsWithTag("Agent")[network.GetID()].GetComponent<BirdController>();
            Debug.Log(bird + " - " + network.GetID());
            bird.ResetBird();
        }
    }

    private void MutateNetworks()
    {
        int eliteIndex = Mathf.RoundToInt(networks.Count * elitistPercentage);
        int crossoverIndex = eliteIndex + Mathf.RoundToInt(networks.Count * crossoverPercentage);
        int mutationIndex = crossoverIndex + Mathf.RoundToInt(networks.Count * mutationPercentage);

        Debug.LogFormat("Elites: {0} Crossover: {1} Mutation: {2}", eliteIndex, crossoverIndex, mutationIndex);

        for(int i = 0; i < networks.Count; i++)
        {
            if (i < eliteIndex) continue; // Save top networks

            // Mutate remaining as copies of top networks
            networks[i] = networks[i - eliteIndex];
            networks[i].Mutate(0, .2f);
        }
    }
}
