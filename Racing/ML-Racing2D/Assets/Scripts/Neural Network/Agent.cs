using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;

public class Agent : MonoBehaviour
{

    private NeuralNetwork network;
    private float[] inputs = new float[5];
    int id;

    private float timeAlive;

    private SpriteRenderer body;
    [SerializeField] public bool dead = false;

    private Population population;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        dead = false;
        timeAlive = 0f;

        body = GetComponentsInChildren<SpriteRenderer>()[1];
        body.color = Random.ColorHSV();

        population = GameObject.FindGameObjectWithTag("Population").GetComponent<Population>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            // Cast a ray straight down.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up);

            // If it hits something...
            if (hit.collider != null)
            {
                // Calculate the distance from the surface and the "error" relative
                // to the floating height.
                float distance = Mathf.Abs(hit.point.y - transform.position.y);
                float heightError = floatHeight - distance;

                // The force is proportional to the height error, but we remove a part of it
                // according to the object's speed.
                float force = liftForce * heightError - rb2D.velocity.y * damping;

                // Apply the force to the rigidbody.
                rb2D.AddForce(Vector3.up * force);
            }


            inputs[0] = transform.position.y; // Bird Y
            inputs[1] = network.GetInput(1);
            inputs[2] = network.GetInput(2);

            //Debug.LogFormat("0: {0} - 1: {1} - 2: {2}", inputs[0], inputs[1], inputs[2]);
            Debug.DrawLine(transform.position, new Vector3(inputs[1], inputs[2], 0), Color.red);

            float[] output = network.FeedForward(inputs);

            //Debug.Log(output[0] + " - " + jumpActivation);

            timeAlive += Time.deltaTime;
            network.SetFitness(timeAlive + score);
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
        this.network = nn;
        this.id = i;
        gameObject.transform.name = "Bird " + i;
    }

    public void SetInput(int i, float value)
    {
        inputs[i] = value;
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
