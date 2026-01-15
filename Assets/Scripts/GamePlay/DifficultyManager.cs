using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager I { get; private set; }

    [Header("Phase (set by WaveManager)")]
    [Range(1, 6)] public int waveIndex = 1;
    public bool isBossPhase = false;

    [Header("Multipliers (X = wave 1..6, Y = multiplier)")]
    [SerializeField] private AnimationCurve enemyBulletSpeed = AnimationCurve.Linear(1, 1.00f, 6, 1.25f);
    [SerializeField] private AnimationCurve enemyMoveSpeed = AnimationCurve.Linear(1, 1.00f, 6, 1.20f);
    [SerializeField] private AnimationCurve enemyFireRate = AnimationCurve.Linear(1, 1.00f, 6, 1.25f);

    [Header("Boss Extras (optional)")]
    [SerializeField] private float bossExtraBulletSpeed = 0.05f;
    [SerializeField] private float bossExtraFireRate = 0.05f;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(gameObject); return; }
        I = this;
        // DontDestroyOnLoad(gameObject);
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

    public float MoveSpeedMult => enemyMoveSpeed.Evaluate(waveIndex);

    public float FireRateMult
    {
        get
        {
            float m = enemyFireRate.Evaluate(waveIndex);
            if (isBossPhase) m *= (1f + bossExtraFireRate);
            return m;
        }
    }

    // Multiply wait-times by this to make enemies fire faster with higher FireRateMult
    public float IntervalMult => 1f / Mathf.Max(0.01f, FireRateMult);
}
