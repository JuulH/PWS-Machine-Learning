using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paddle : MonoBehaviour
{

    private float xInput;
    [SerializeField] private float speed;
    [SerializeField] private float xWidth;

    [SerializeField] private Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = startPos;
    }

    // Update is called once per frame
    void Update()
    {
        // Move paddle based on horizontal (AD/<>) input
        xInput = Input.GetAxis("Horizontal");
        transform.position += Vector3.right * xInput * speed * Time.deltaTime;

        // Restrict paddle movement to bounds
        if (transform.position.x < -xWidth)
        {
            transform.position = new Vector3(-xWidth, transform.position.y);
        } else if (transform.position.x > xWidth)
        {
            transform.position = new Vector3(xWidth, transform.position.y);
        }
    }

    public void ResetPaddle()
    {
        transform.position = startPos;
    }
}
