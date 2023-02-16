using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Agent : MonoBehaviour
{

    private TopDownCarController car;
    private NeuralNetwork network;
    private float[] inputs = new float[5];
    int id;

    [SerializeField] private int[] layers;

    private float timeAlive;

    private SpriteRenderer body;
    [SerializeField] public bool dead = false;

    private int score;

    private int borderLayer;
    [SerializeField] private int numRays = 5;
    [SerializeField] private float maxAngle = 90f;
    private float spreadAngle;
    [SerializeField] private LineRenderer line;

    [SerializeField] private Vector2 startPos;


    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        dead = false;
        timeAlive = 0f;

        borderLayer = LayerMask.GetMask("Border");
        spreadAngle = maxAngle / (numRays - 1);

        line.positionCount = numRays * 2;

        car = GetComponent<TopDownCarController>();
        inputs = new float[numRays];
        layers[0] = numRays;
        network = new NeuralNetwork(layers);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            transform.position = startPos;
            network = new NeuralNetwork(layers);
        }

        if (!dead)
        {
            for(int i = 0; i < numRays; i++)
            {
                // Calculate the angle of the current ray
                float angle = (-maxAngle / 2) + (i * spreadAngle);

                // Calculate the direction of the current ray
                Vector2 direction = Quaternion.Euler(0f, 0f, angle) * transform.up;

                // Cast the ray and draw the debug line
                RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 50f, borderLayer);
                if (hit.collider != null)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.red);

                    line.SetPosition(i * 2, transform.position);
                    line.SetPosition(i * 2 + 1, hit.point);

                    inputs[i] = hit.distance;
                } else
                {
                    line.SetPosition(i * 2, transform.position);
                    line.SetPosition(i * 2 + 1, transform.position);

                    inputs[i] = 9999;
                }
            }

            Debug.LogFormat("0: {0} - 1: {1} - 2: {2} - 3: {3} - 4: {4}", inputs[0], inputs[1], inputs[2], inputs[3], inputs[4]);

            float[] output = network.FeedForward(inputs);

            Vector2 inputVector = new Vector2(output[0], output[1]);
            Debug.DrawLine(transform.position, inputVector, Color.green);

            car.SetInputVector(inputVector);

            timeAlive += Time.deltaTime;
            //network.SetFitness(timeAlive + score);
        } else
        {
            //visionLine.enabled = false;
        }
    }

    public void ResetAgent()
    {
        // test
    }

    public void InitAgent(NeuralNetwork nn, int i)
    {
        //this.network = nn;
        this.id = i;
        gameObject.transform.name = "Bird " + i;
    }

    public void SetInput(int i, float value)
    {
        inputs[i] = value;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //dead = true;
        //this.gameObject.SetActive(false);

        transform.position = startPos;
        network = new NeuralNetwork(layers);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Pipe"))
        {
            score++;
            //population.SetScore(score);
        }
    }

    public void ResetBird()
    {
        transform.position = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;
        dead = false;
        timeAlive = 0f;
        score = 0;
    }
}
