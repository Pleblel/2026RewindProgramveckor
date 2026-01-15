using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    [System.Serializable]
    public class WaveDef
    {
        public string name = "Wave";
        public int killsToClear = 20;

        [Header("Side")]
        public bool sideEnabled = true;
        public Vector2 sideBurstInterval = new Vector2(2.2f, 4.0f);
        public Vector2Int sideBurstAmount = new Vector2Int(3, 8);
        public float sideBurstGap = 0.5f;
        public float sideYOffsetMin = -2f;
        public float sideYOffsetMax = 2f;
        public int sideMaxAlive = 12;

        [Header("Chase")]
        public bool chaseEnabled = true;
        public Vector2 chaseBurstInterval = new Vector2(2.0f, 3.5f);
        public Vector2Int chaseBurstAmount = new Vector2Int(2, 6);
        public float chaseBurstGap = 0.5f;
        public int chaseMaxAlive = 10;

        [Header("Stationary (spawn once at kill threshold)")]
        public bool stationaryEnabled = false;
        public int stationaryAmount = 2;
        public int stationaryMaxAlive = 2;
        public int spawnStationaryAtKill = -1;
    }

    [Header("Refs")]
    [SerializeField] private EnemySpawner spawner;
    [SerializeField] private CardRewardManager cardRewards;

    [Header("Waves (must be 6)")]
    [SerializeField] private List<WaveDef> waves = new List<WaveDef>(6);

    [Header("Bosses")]
    [SerializeField] private Primadon midBossPrefab;
    [SerializeField] private Primadon finalBossPrefab;
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Between phases")]
    [SerializeField] private float intermissionSeconds = 2.0f;
    [SerializeField] private bool waitForCleanupBeforeBoss = true;

    private int killsThisWave = 0;
    private bool countingKills = false;
    private bool bossActive = false;

    private void Awake()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
        if (cardRewards == null) cardRewards = FindFirstObjectByType<CardRewardManager>();
    }

    private void OnEnable()
    {
        EnemyEntity.OnAnyEnemyKilled += OnAnyEnemyKilled;
    }

    private void OnDisable()
    {
        EnemyEntity.OnAnyEnemyKilled -= OnAnyEnemyKilled;
    }

    private void OnAnyEnemyKilled(EnemyEntity e)
    {
        if (!countingKills) return;
        if (bossActive) return;
        if (e is Primadon) return;
        killsThisWave++;
    }

    private void Start()
    {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        if (DifficultyManager.I == null)
            Debug.LogWarning("No DifficultyManager found. Scaling won't apply.");

        // Waves 1-3
        for (int i = 0; i < 3; i++)
        {
            DifficultyManager.I?.SetPhase(i + 1, false);
            yield return RunWave(waves[i]);
            yield return new WaitForSeconds(intermissionSeconds);
        }

        // Midboss after wave 3
        DifficultyManager.I?.SetPhase(3, true);
        yield return RunBoss(midBossPrefab);

        yield return new WaitForSeconds(intermissionSeconds);

        // Waves 4-6
        for (int i = 3; i < 6; i++)
        {
            DifficultyManager.I?.SetPhase(i + 1, false);
            yield return RunWave(waves[i]);
            yield return new WaitForSeconds(intermissionSeconds);
        }

        // Final boss after wave 6
        DifficultyManager.I?.SetPhase(6, true);
        yield return RunBoss(finalBossPrefab);

        // TODO: win screen
    }

    private IEnumerator RunWave(WaveDef w)
    {
        // ✅ CARD PICK AT START OF WAVE
        if (cardRewards != null)
        {
            // This opens UI + sets Time.timeScale = 0 inside CardRewardManager
            cardRewards.OpenCardChoice();

            // Wait until player picks a card (PickCard sets timeScale back to 1)
            yield return new WaitUntil(() => Time.timeScale > 0f);

            // Refresh barrier etc at the start of the wave AFTER picking
            cardRewards.OnWaveStart();
        }

        killsThisWave = 0;
        countingKills = true;
        bossActive = false;

        float nextSide = Time.time + Random.Range(w.sideBurstInterval.x, w.sideBurstInterval.y);
        float nextChase = Time.time + Random.Range(w.chaseBurstInterval.x, w.chaseBurstInterval.y);

        bool spawnedStationary = false;

        while (killsThisWave < w.killsToClear)
        {
            spawner.CleanupNulls();

            // Stationary trigger (once)
            if (w.stationaryEnabled && !spawnedStationary && w.spawnStationaryAtKill >= 0 && killsThisWave >= w.spawnStationaryAtKill)
            {
                if (spawner.AliveStationaryCount < w.stationaryMaxAlive)
                {
                    spawner.SpawnStationary(w.stationaryAmount);
                    spawnedStationary = true;
                }
            }

            // Side bursts
            if (w.sideEnabled && Time.time >= nextSide && spawner.AliveSideCount < w.sideMaxAlive)
            {
                int amt = Random.Range(w.sideBurstAmount.x, w.sideBurstAmount.y + 1);
                yield return StartCoroutine(spawner.SpawnSideBurst(amt, w.sideBurstGap, w.sideYOffsetMin, w.sideYOffsetMax));
                nextSide = Time.time + Random.Range(w.sideBurstInterval.x, w.sideBurstInterval.y);
            }

            // Chase bursts
            if (w.chaseEnabled && Time.time >= nextChase && spawner.AliveChaseCount < w.chaseMaxAlive)
            {
                int amt = Random.Range(w.chaseBurstAmount.x, w.chaseBurstAmount.y + 1);
                yield return StartCoroutine(spawner.SpawnChaseBurst(amt, w.chaseBurstGap));
                nextChase = Time.time + Random.Range(w.chaseBurstInterval.x, w.chaseBurstInterval.y);
            }

            yield return null;
        }

        countingKills = false;

        // Optional: wait for screen to clear
        while (spawner.AliveSideCount > 0 || spawner.AliveChaseCount > 0 || spawner.AliveStationaryCount > 0)
            yield return null;
    }

    private IEnumerator RunBoss(Primadon bossPrefab)
    {
        if (bossPrefab == null || bossSpawnPoint == null) yield break;

        countingKills = false;
        bossActive = true;

        if (waitForCleanupBeforeBoss)
        {
            spawner.CleanupNulls();
            while (spawner.AliveSideCount > 0 || spawner.AliveChaseCount > 0 || spawner.AliveStationaryCount > 0)
                yield return null;
        }

        Primadon boss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

        while (boss != null)
            yield return null;

        bossActive = false;
    }
}
