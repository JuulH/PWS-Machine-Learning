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

    // Neural network config
    [SerializeField] private int[] layers = new int[] { 6, 8, 8, 1 };
    private List<NeuralNetwork> networks;
    private List<GameObject> cars;

    //[SerializeField] private StatsManager stats;

    // Mutation
    private float elitistPercentage = 0.1f;
    private float crossoverPercentage = 0.25f;
    private float mutationPercentage = 0.65f;

    [SerializeField] private bool trackFittest;

    [SerializeField] private float timeScale;

    private float runtime;

    [SerializeField] private int numRays = 5;
    [SerializeField] private float maxAngle = 90f;
    private float spreadAngle;

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

        //if (Input.GetKeyDown(KeyCode.Return))
        //{
        //    trackFittest = !trackFittest;
        //    if (trackFittest)
        //    {
        //        foreach (GameObject bird in birds)
        //        {
        //            if (!bird.activeInHierarchy) continue;

        //            bird.transform.GetChild(0).gameObject.SetActive(false);
        //            birds[networks[0].GetID()].transform.GetChild(0).gameObject.SetActive(true);
        //        }
        //    } else
        //    {
        //        foreach (GameObject bird in birds)
        //        {
        //            bird.transform.GetChild(0).gameObject.SetActive(true);
        //        }
        //    }
        //}

        agents = GameObject.FindGameObjectsWithTag("Agent");
        alive = agents.Length;

        UpdateStats();

        if(training)
        {
            //if(trackFittest)
            //{
            //    if (!birds[networks[0].GetID()].activeInHierarchy)
            //    {
            //        foreach (GameObject bird in birds)
            //        {
            //            if (bird.activeInHierarchy)
            //            {
            //                bird.transform.GetChild(0).gameObject.SetActive(true);
            //                break;
            //            }
            //        }
            //    }
            //}

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

                Debug.LogFormat("Generation {0} - Fitness: {1}, Average Fitness: {2}", generation, lastFitness, averageFitness);

                //stats.Write(new float[] {generation, lastFitness, averageFitness, currentScore, runtime});

                MutateNetworks();
                ResetNetworks();

                //if (trackFittest)
                //{
                //    foreach (GameObject bird in birds)
                //    {
                //        bird.transform.GetChild(0).gameObject.SetActive(false);
                //    }
                //    birds[networks[0].GetID()].transform.GetChild(0).gameObject.SetActive(true);
                //}

                // Data
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
            "Alive: {4}/{5}\n" +
            "Timescale: {6}x",
            generation, recordFitness, lastFitness, averageFitness, alive, populationSize, timeScale);
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
            car.numRays = numRays;
            car.maxAngle = maxAngle;
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
            car.Reset();
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
