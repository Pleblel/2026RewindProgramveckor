using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    public int score;
    public float scoreMultiplier = 1.0f;
    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        // If you forgot to assign it, try auto-find on same object
        if (scoreText == null)
            scoreText = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        score = 0; // change if you want
        RefreshUI();
    }

    public void AddScore(int s)
    {
        score += Mathf.RoundToInt(s * scoreMultiplier);
        RefreshUI();
    }

    public bool TrySpendScore(int amount)
    {
        if (score < amount) return false;
        score -= amount;
        RefreshUI();
        return true;
    }

    private void RefreshUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score;
    }
}
