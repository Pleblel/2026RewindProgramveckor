using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int score;
    public float scoreMultiplier = 1.0f;
    [SerializeField] TextMeshProUGUI scoreText;


    private void Start()
    {
        StartGame();
    }
    void StartGame()
    {
        score = 100000;
    }

    public void AddScore(int s)
    {
        score += (int)(s * scoreMultiplier);
        
    }

    private void Update()
    {
        scoreText.text = "Score: " + score;

    }
}
