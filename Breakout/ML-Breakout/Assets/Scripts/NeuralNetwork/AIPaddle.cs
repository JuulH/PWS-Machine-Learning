using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class AIPaddle : MonoBehaviour
{

    [SerializeField] private float speed = 9;
    [SerializeField] private float xWidth;
    [SerializeField] private Vector2 startPos;

    private bool initialized = false;
    [SerializeField] private Ball ball;

    // Neural network
    private NeuralNetwork network;
    private float[] inputs = new float[5];
    private int[] layers = new int[] { 6, 6, 6, 1 };

    // Start is called before the first frame update
    void Start()
    {
        speed = 9;
        xWidth = 4.625f;
        initialized = false;
        startPos = new Vector2(0, -4);
        transform.position = startPos;
        //Init(new NeuralNetwork(layers));
        network = new NeuralNetwork(layers);
        initialized = true;
        Debug.Log(network);
    }

    // Update is called once per frame
    void Update()
    {
        if (initialized)
        {
            // Set inputs to neural network
            inputs[0] = transform.position.x;
            inputs[1] = ball.transform.position.x;
            inputs[2] = ball.transform.position.y;
            inputs[3] = ball.dir.x;
            inputs[4] = ball.dir.y;

            float[] output = network.FeedForward(inputs);

            float moveDir = (float) System.Math.Tanh(output[0]);
            transform.position += Vector3.right * moveDir * speed * Time.deltaTime;

            // Restrict paddle movement to bounds
            if (transform.position.x < -xWidth)
            {
                transform.position = new Vector3(-xWidth, transform.position.y);
            }
            else if (transform.position.x > xWidth)
            {
                transform.position = new Vector3(xWidth, transform.position.y);
            }
        }
    }

    public void Init(NeuralNetwork nn)
    {
        this.network = nn;
        initialized = true;
    }
}
