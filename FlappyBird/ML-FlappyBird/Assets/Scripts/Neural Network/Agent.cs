using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Agent : MonoBehaviour
{

    private NeuralNetwork network;
    private float[] inputs = new float[5];
    int id;

    float jumpActivation;

    private BirdController bird;

    private float timeAlive;

    private SpriteRenderer body;
    private LineRenderer visionLine;

    // Start is called before the first frame update
    void Start()
    {
        bird = GetComponent<BirdController>();
        timeAlive = 0f;

        body = bird.GetComponentsInChildren<SpriteRenderer>()[1];
        body.color = Random.ColorHSV();

        visionLine = bird.GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!bird.dead)
        {
            inputs[0] = transform.position.y; // Bird Y
            inputs[1] = network.GetInput(1);
            inputs[2] = network.GetInput(2);

            //Debug.LogFormat("0: {0} - 1: {1} - 2: {2}", inputs[0], inputs[1], inputs[2]);
            Debug.DrawLine(transform.position, new Vector3(inputs[1], inputs[2], 0), Color.red);
            visionLine.SetPosition(0, transform.position);
            visionLine.SetPosition(1, new Vector2(inputs[1], inputs[2]));

            float[] output = network.FeedForward(inputs);
            jumpActivation = (float)System.Math.Tanh(output[0]);

            Debug.Log(output[0] + " - " + jumpActivation);

            if (jumpActivation > 0)
            {
                bird.Jump();
            }

            timeAlive += Time.deltaTime;
            network.SetFitness(timeAlive);
        } else
        {
            visionLine.enabled = false;
        }
    }

    public void InitAgent(NeuralNetwork nn, int i)
    {
        this.network = nn;
        this.id = i;
        //bird.transform.name = "Bird " + i;
    }

    public void SetInput(int i, float value)
    {
        inputs[i] = value;
    }
}
