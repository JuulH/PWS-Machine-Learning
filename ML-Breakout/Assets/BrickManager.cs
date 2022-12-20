using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{

    [SerializeField] private int rows;
    [SerializeField] private int columns;

    private float bricksMiddle;

    [SerializeField] private GameObject brickPrefab;

    [SerializeField] private Color[] brickColors;

    // Start is called before the first frame update
    void Start()
    {
        SetBricks();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ClearBricks()
    {
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void SetBricks()
    {
        GameObject brick = brickPrefab;
        bricksMiddle = transform.position.y;

        // Get brick dimensions
        float brickWidth = brick.transform.localScale.x;
        float brickHeight = brick.transform.localScale.y;

        // Total wall dimensions
        float totalWidth = brickWidth * columns;
        float totalHeight = brickHeight * rows;

        float leftX, topY;
        if (rows > 1)
        {
            leftX = (-totalWidth * 0.5f) + (brickWidth * 0.5f);
        } else
        {
            leftX = 0;
        }

        if (columns > 1)
        {
            topY = bricksMiddle + ((totalHeight * 0.5f) - (brickHeight * 0.5f));
        } else
        {
            topY = bricksMiddle;
        }

        float currentX = leftX;
        float currentY = topY;

        // Instantiate bricks
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject newBrick = Instantiate(brick, new Vector2(currentX, currentY), Quaternion.identity, transform);
                SpriteRenderer brickSprite = newBrick.GetComponent<SpriteRenderer>();
                brickSprite.color = brickColors[i];
                currentX += brickWidth;
            }
            currentY -= brickHeight;
            currentX = leftX;
        }
    }
}
