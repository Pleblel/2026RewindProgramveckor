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
    [SerializeField] private AnimationCurve enemyFireRate = AnimationCurve.Linear(1, 1.00f, 6, 1.25f);

    [Header("Boss Bonus (optional)")]
    [SerializeField] private float bossExtraBulletSpeed = 0.05f; // +5% during bosses
    [SerializeField] private float bossExtraFireRate = 0.05f; // +5% during bosses

    [Header("Cursed Rank Bonuses (added by cards)")]
    [Tooltip("Added as +% (0.12 = +12%)")]
    public float rankBulletSpeedBonus = 0f;

    [Tooltip("Added as +% (0.15 = +15%)")]
    public float rankFireRateBonus = 0f;

    [Tooltip("Added as +% (0.15 = +15%). You must use SpawnMult in your spawner.")]
    public float rankSpawnBonus = 0f;

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

    // ---- Methods your CardRewardManager expects ----
    public void AddRankBulletSpeed(float add) => rankBulletSpeedBonus += add;
    public void AddRankFireRate(float add) => rankFireRateBonus += add;
    public void AddRankSpawn(float add) => rankSpawnBonus += add;

    // ---- Multipliers used by enemies / patterns ----
    public float BulletSpeedMult
    {
        get
        {
            float m = enemyBulletSpeed.Evaluate(waveIndex);
            m *= (1f + Mathf.Max(0f, rankBulletSpeedBonus));
            if (isBossPhase) m *= (1f + Mathf.Max(0f, bossExtraBulletSpeed));
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
            m *= (1f + Mathf.Max(0f, rankFireRateBonus));
            if (isBossPhase) m *= (1f + Mathf.Max(0f, bossExtraFireRate));
            return m;
        }
    }

    // Use this when you do WaitForSeconds for enemy shooting/patterns:
    // Higher FireRateMult => shorter waits
    public float IntervalMult => 1f / Mathf.Max(0.01f, FireRateMult);

    // Use this in your spawner/wave system:
    // spawnCount *= SpawnMult OR spawnDelay /= SpawnMult
    public float SpawnMult => 1f + Mathf.Max(0f, rankSpawnBonus);
}
