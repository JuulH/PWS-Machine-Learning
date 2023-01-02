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

    private float timeAlive;

    private SpriteRenderer body;
    private LineRenderer visionLine;

    // Bird
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody2D rb;
    [SerializeField] public bool dead = false;

    private Population population;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        rb = GetComponent<Rigidbody2D>();
        dead = false;
        timeAlive = 0f;

        body = GetComponentsInChildren<SpriteRenderer>()[1];
        body.color = Random.ColorHSV();

        visionLine = GetComponent<LineRenderer>();

        population = GameObject.FindGameObjectWithTag("Population").GetComponent<Population>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
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

            //Debug.Log(output[0] + " - " + jumpActivation);

            if (jumpActivation > 0)
            {
                Jump();
            }

            timeAlive += Time.deltaTime;
            network.SetFitness(timeAlive + score);
        } else
        {
            visionLine.enabled = false;
        }
    }

    public void InitAgent(NeuralNetwork nn, int i)
    {
        this.network = nn;
        this.id = i;
        gameObject.transform.name = "Bird " + i;
    }

    public void SetInput(int i, float value)
    {
        inputs[i] = value;
    }

    public void Jump()
    {
        rb.velocity = Vector2.up * jumpForce;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        dead = true;
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Pipe"))
        {
            score++;
            population.SetScore(score);
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
