using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private float jumpForce = 5f;
    private Rigidbody2D rb;

    [SerializeField] public bool dead = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        dead = false;
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

    private void StartGame()
    {
        rb.simulated = true;
    }

    public void ResetBird()
    {
        transform.position = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;
        dead = false;
    }

}
