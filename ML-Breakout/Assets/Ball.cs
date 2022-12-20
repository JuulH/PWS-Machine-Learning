using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{

    [SerializeField] private float speed;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private float maxAngle;

    [SerializeField] private Vector3 startPos;
    [SerializeField] private float failHeight;

    [SerializeField] private GameManager gameManager;

    private bool hitPaddle;

    // Start is called before the first frame update
    void Start()
    {
        ResetBall();
        hitPaddle = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ResetBall();
        }

        if(transform.position.y < failHeight)
        {
            gameManager.OnFail();
        }
    }

    public void ResetBall()
    {
        hitPaddle = true;

        transform.position = startPos;

        float leftAngle = (90f + (maxAngle / 2)) * Mathf.Deg2Rad;
        float rightAngle = (90f - (maxAngle / 2)) * Mathf.Deg2Rad;

        float angle = Random.Range(leftAngle, rightAngle);

        Vector3 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        rb.velocity = dir * speed;
        Debug.DrawLine(transform.position, transform.position + dir * speed, Color.red, 5f);

        Debug.Log(string.Format("Left limit: {0}, Angle: {1}, Right limit: {2}", leftAngle * Mathf.Rad2Deg, angle * Mathf.Rad2Deg, rightAngle * Mathf.Rad2Deg));
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(leftAngle), Mathf.Sin(leftAngle)) * 4, Color.white, 5f); // Left limit
        Debug.DrawLine(transform.position, transform.position + new Vector3(Mathf.Cos(rightAngle), Mathf.Sin(rightAngle)) * 4, Color.white, 5f); // Right limit
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Brick"))
        {
            Destroy(collision.gameObject);
            if (!hitPaddle)
            {
                gameManager.MultiplierUp();
            }
            gameManager.ScoreUp();
            hitPaddle = false;
        } else if (collision.gameObject.CompareTag("Paddle"))
        {
            Vector2 distanceToPaddle = transform.position - collision.transform.position;
            rb.velocity = distanceToPaddle.normalized * speed;
            hitPaddle = true;
            gameManager.ResetMultiplier();
        }
    }
}
