using UnityEngine;

public class CarController : MonoBehaviour
{
    // variables for movement
    public float acceleration = 20f;
    public float turnSpeed = 10f;
    public float maxSpeed = 50f;
    public float driftFactorSticky = 0.9f;
    public float driftFactorSlippy = 1f;
    public float maxStickyVelocity = 2f;

    // variables for drifting
    public bool drifting = false;
    public float driftAngle = 10f;
    public float driftSmoothing = 7f;
    public float driftVelocity = 5f;
    public float minDriftVelocity = 10f;
    public float maxDriftVelocity = 50f;

    // private variables
    private float currentDriftVelocity;
    private float currentSpeed;
    private Rigidbody rb;
    private float inputSteer;
    private float inputAcceleration;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // get input for movement
        inputSteer = Input.GetAxis("Horizontal");
        inputAcceleration = Input.GetAxis("Vertical");
    }

    void FixedUpdate()
    {
        // move the car forward
        rb.AddForce(transform.forward * acceleration * inputAcceleration);

        // limit the speed of the car
        currentSpeed = rb.velocity.magnitude;
        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        // turn the car based on input
        transform.Rotate(Vector3.up, turnSpeed * inputSteer * Mathf.Min(1, rb.velocity.sqrMagnitude / 10));

        // handle drifting
        if (drifting)
        {
            // calculate the drift angle based on current velocity
            float driftAngleAmount = driftAngle * (currentDriftVelocity / maxDriftVelocity);

            // rotate the car towards the drift angle
            Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + driftAngleAmount, 0);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * driftSmoothing);

            // reduce the drift velocity over time
            currentDriftVelocity *= driftFactorSlippy;
            if (currentDriftVelocity < minDriftVelocity)
            {
                drifting = false;
            }
        }
        else
        {
            // if not drifting, reduce the current velocity to stick to the ground
            currentDriftVelocity *= driftFactorSticky;
            if (currentDriftVelocity < maxStickyVelocity)
            {
                currentDriftVelocity = 0f;
            }
        }

        // start drifting if the player presses the drift button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            drifting = true;
            currentDriftVelocity = driftVelocity;
        }
    }
}