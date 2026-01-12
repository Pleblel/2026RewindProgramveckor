using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score : MonoBehaviour
{
    protected int score;
    public float scoreMultiplier = 1.0f;
    void StartGame()
    {
        score = 1000;
    }

    public void AddScore(int s)
    {
        score += (int)(s * scoreMultiplier);
    }
}
