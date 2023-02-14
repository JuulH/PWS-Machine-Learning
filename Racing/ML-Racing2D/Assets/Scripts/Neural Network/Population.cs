using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Population : MonoBehaviour
{

    [SerializeField] private bool training;

    [SerializeField] private int populationSize;
    [SerializeField] private int generation;
    [SerializeField] private int alive;

    private GameObject[] agents;

    [SerializeField] private GameObject agentPrefab;

    [SerializeField] private TMP_Text statsText;
    [SerializeField] private float recordFitness;
    [SerializeField] private float lastFitness;
    [SerializeField] private float averageFitness;
    [SerializeField] private int highScore;
    [SerializeField] private int currentScore;
    public bool updateScore;

    // Neural network config
    [SerializeField] private int[] layers = new int[] { 6, 8, 8, 1 };
    private List<NeuralNetwork> networks;
    private List<GameObject> cars;

    [SerializeField] private StatsManager stats;

    // Mutation
    private float elitistPercentage = 0.1f;
    private float crossoverPercentage = 0.25f;
    private float mutationPercentage = 0.65f;
    [SerializeField] private bool trackFittest;

    [SerializeField] private float timeScale;

    private float runtime;

    // Start is called before the first frame update
    void Start()
    {
        InitNetworks();
        //stats.CreateCSV(DateTime.Now.ToString("yyyy-MM-dd") + "_" + stats.GetRuns().ToString());
        //stats.UpdateRuns();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            timeScale *= 2;
            Time.timeScale = timeScale;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            timeScale /= 2;
            Time.timeScale = timeScale;
        }

        agents = GameObject.FindGameObjectsWithTag("Agent");
        alive = agents.Length;

        UpdateStats();

        if(training)
        {

            if (alive <= 0)
            {
                // Sort networks by fitness descending
                networks.Sort();
                lastFitness = networks[0].GetFitness();

                if (lastFitness > recordFitness) recordFitness = lastFitness;

                float tally = 0f;
                foreach (NeuralNetwork network in networks)
                {
                    tally += network.GetFitness();
                }
                averageFitness = tally / populationSize;

                Debug.LogFormat("Generation {0} - Fitness: {1}, Average Fitness: {2}, Score: {3}", generation, lastFitness, averageFitness, currentScore);

                stats.Write(new float[] {generation, lastFitness, averageFitness, currentScore, runtime});

                MutateNetworks();
                ResetNetworks();
                // Data
                currentScore = 0;
                generation++;
                runtime = 0f;
            } else
            {
                runtime += Time.deltaTime;
            }
        }
    }

    private void UpdateStats()
    {
        statsText.text = string.Format(
            "Generation: {0}\n" +
            "Highest Fitness: {1}\n" +
            "Last Fitness: {2}\n" +
            "Average Fitness: {3}\n" +
            "Highest Score: {4}\n" +
            "Current Score: {5}\n" +
            "Alive: {6}/{7}\n" +
            "Timescale: {8}x",
            generation, recordFitness, lastFitness, averageFitness, highScore, currentScore, alive, populationSize, timeScale);
    }

    public void ScoreUp()
    {
        if (updateScore)
        {
            currentScore++;
            if (currentScore > highScore) highScore = currentScore;
            updateScore = false;
        }
    }

    public void SetScore(int score)
    {
        currentScore = score;
        if (currentScore > highScore) highScore = currentScore;
    }

    // Initialize network with random weights and biases
    private void InitNetworks()
    {
        networks = new List<NeuralNetwork>();
        cars = new List<GameObject>();

        for(int i = 0; i < populationSize; i++)
        {
            NeuralNetwork network = new NeuralNetwork(layers);
            network.SetID(i);
            networks.Add(network);

            Agent car = Instantiate(agentPrefab, new Vector3(0, 0, 0), Quaternion.identity).GetComponent<Agent>();
            car.InitAgent(network, i);
            cars.Add(car.gameObject);
        }
    }

    // Reset networks for new generation
    private void ResetNetworks()
    {
        foreach(GameObject car in cars)
        {
            car.SetActive(true);
        }
        foreach(NeuralNetwork network in networks)
        {
            Agent car = cars[network.GetID()].GetComponent<Agent>();
            // Debug.Log(bird + " - " + network.GetID());
            car.ResetAgent();
        }
    }

    private void MutateNetworks()
    {
        int eliteIndex = Mathf.RoundToInt(networks.Count * elitistPercentage);
        int crossoverIndex = eliteIndex + Mathf.RoundToInt(networks.Count * crossoverPercentage);
        int mutationIndex = crossoverIndex + Mathf.RoundToInt(networks.Count * mutationPercentage);

        // Debug.LogFormat("Elites: {0} Crossover: {1} Mutation: {2}", eliteIndex, crossoverIndex, mutationIndex);

        for(int i = 0; i < networks.Count; i++)
        {
            if (i < eliteIndex) continue; // Save top networks

            int id = networks[i].GetID();

            // Mutate remaining as copies of top networks
            // networks[i] = networks[(i - eliteIndex) % eliteIndex];
            networks[i].Copy(networks[(i - eliteIndex) % eliteIndex]);
            networks[i].SetID(id);
            networks[i].Mutate(0, .2f);
        }
    }
}
