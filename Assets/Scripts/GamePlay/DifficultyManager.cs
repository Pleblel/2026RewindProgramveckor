using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager I { get; private set; }

    [Header("Current Phase")]
    [Range(1, 6)] public int waveIndex = 1;
    public bool isBossPhase = false;

    [Header("Scaling Curves (X = wave number 1..6, Y = multiplier)")]
    [SerializeField] private AnimationCurve enemyBulletSpeed = AnimationCurve.Linear(1, 1.00f, 6, 1.25f);
    [SerializeField] private AnimationCurve enemyMoveSpeed = AnimationCurve.Linear(1, 1.00f, 6, 1.20f);

    // Higher = enemies shoot more often / patterns tick faster
    [SerializeField] private AnimationCurve enemyFireRate = AnimationCurve.Linear(1, 1.00f, 6, 1.25f);

    [Header("Boss Bonus (optional)")]
    [SerializeField] private float bossExtraBulletSpeed = 0.05f; // +5% during bosses
    [SerializeField] private float bossExtraFireRate = 0.05f; // +5% during bosses

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        // DontDestroyOnLoad(gameObject); // enable if you change scenes
    }

    public void SetPhase(int wave, bool boss)
    {
        waveIndex = Mathf.Clamp(wave, 1, 6);
        isBossPhase = boss;
    }

    public float BulletSpeedMult
    {
        get
        {
            float m = enemyBulletSpeed.Evaluate(waveIndex);
            if (isBossPhase) m *= (1f + bossExtraBulletSpeed);
            return m;
        }
    }

    public float MoveSpeedMult
    {
        get
        {
            return enemyMoveSpeed.Evaluate(waveIndex);
        }
    }

    public float FireRateMult
    {
        get
        {
            float m = enemyFireRate.Evaluate(waveIndex);
            if (isBossPhase) m *= (1f + bossExtraFireRate);
            return m;
        }
    }

    // Convert "fire rate multiplier" into "wait time multiplier"
    // Example: FireRateMult=1.25 => waits become 1/1.25 = 0.8 (faster attacks)
    public float IntervalMult => 1f / Mathf.Max(0.01f, FireRateMult);
}
