using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    private float score;
    private float multiplier;

    [SerializeField] private TMP_Text scoreText;

    [SerializeField] private Paddle paddle;
    [SerializeField] private Ball ball;
    [SerializeField] private BrickManager brickManager;

    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        multiplier = 1;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format("Score: {0}", score.ToString());
    }

    public void ScoreUp()
    {
        score += multiplier;
    }

    public void MultiplierUp()
    {
        multiplier += 0.25f;
    }

    public void ResetMultiplier()
    {
        multiplier = 1f;
    }

    public void OnFail()
    {
        brickManager.ClearBricks();
        brickManager.SetBricks();
        score = 0;
        multiplier = 1;
        paddle.ResetPaddle();
        ball.ResetBall();
    }
}
