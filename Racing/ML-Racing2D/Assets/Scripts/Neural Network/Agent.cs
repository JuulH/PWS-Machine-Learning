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
    private float timeSinceTarget;

    private SpriteRenderer body;
    [SerializeField] public bool dead = false;

    private int borderLayer;
    [SerializeField] public int numRays = 5;
    [SerializeField] public float maxAngle = 90f;
    private float spreadAngle;
    [SerializeField] private LineRenderer line;

    [SerializeField] private Vector2 startPos;
    [SerializeField] private int ctarget = 0;
    [SerializeField] private int target = 0;

    private int maxTargets = 25;

    // Start is called before the first frame update
    void Start()
    {
        dead = false;
        timeAlive = 0f;

        borderLayer = LayerMask.GetMask("Border");
        spreadAngle = maxAngle / (numRays - 1);

        line.positionCount = numRays * 2;
        car = GetComponent<TopDownCarController>();
        body = transform.GetChild(0).GetComponent<SpriteRenderer>();
        body.color = Random.ColorHSV();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Reset();
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

            //Debug.LogFormat("0: {0} - 1: {1} - 2: {2} - 3: {3} - 4: {4}", inputs[0], inputs[1], inputs[2], inputs[3], inputs[4]);

            float[] output = network.FeedForward(inputs);

            Vector2 inputVector = new Vector2(output[0], output[1]);
            Debug.DrawLine(transform.position, inputVector, Color.green);

            car.SetInputVector(inputVector);

            timeAlive += Time.deltaTime;
            timeSinceTarget += Time.deltaTime;
            Debug.Log(network.fitness);
        } else
        {
            //visionLine.enabled = false;
        }
    }

    public void InitAgent(NeuralNetwork nn, int i)
    {
        this.network = nn;
        this.id = i;
        gameObject.transform.name = "Car " + i;

        inputs = new float[network.layers[0]];
    }

    public void SetInput(int i, float value)
    {
        inputs[i] = value;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Border"))
        {
            dead = true;
            this.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Target"))
        {
            target = int.Parse(collision.gameObject.name);
            float multiplier = 1 / (timeSinceTarget + 1);
            if (target == ctarget + 1)
            {
                timeSinceTarget = 0;
                ctarget = target;
                network.fitness += 1 * multiplier;
            }
            else
            {
                network.fitness -= 1 * multiplier;
                timeSinceTarget = 0;
            }

            Debug.Log("Hit target! " + target + ", Multiplier: " + multiplier + ", Fitness: " + network.fitness);

            if (ctarget >= maxTargets)
            {
                ctarget = 0;
                target = 0;
                timeAlive = 0f;
            }
        }
    }

    public void Reset()
    {
        transform.position = startPos;
        transform.rotation = Quaternion.identity;
        timeAlive = 0f;
        timeSinceTarget = 0f;
        ctarget = 0;
        target = 0;
        dead = false;
        car.Reset();
    }
}
