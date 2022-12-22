using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{

    [SerializeField] private float speed = 5f;
    private bool wasVisible;
    private SpriteRenderer childSprite;
    public float pipeMargin;

    // Start is called before the first frame update
    void Start()
    {
        wasVisible = false;
        childSprite = gameObject.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        // Move pipe to left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Destroy pipe if it goes off-screen
        if (childSprite.isVisible)
        {
            wasVisible = true;
        } else
        {
            // Check to make sure it doesn't destroy the pipe before it reaches the screen
            if (wasVisible)
            {
                Destroy(gameObject);
            }
        }
    }

    public void IncreaseSpeed(float increase)
    {
        speed += increase;
    }

    public void SetMargin(float margin)
    {
        pipeMargin = margin;
        Transform topPipe = transform.GetChild(0);
        Transform bottomPipe = transform.GetChild(0);
        float pipeSizeY = topPipe.GetComponent<SpriteRenderer>().bounds.size.y;
        topPipe.position = new Vector2(0, margin / 2 +  pipeSizeY / 2);
        bottomPipe.position = new Vector2(0, -margin / 2 + -pipeSizeY / 2);
    }
}
