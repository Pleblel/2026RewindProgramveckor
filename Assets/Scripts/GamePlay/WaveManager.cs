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

        [Header("Side enemies")]
        public bool sideEnabled = true;
        public Vector2 sideBurstInterval = new Vector2(2.2f, 4.0f);
        public Vector2Int sideBurstAmount = new Vector2Int(3, 8);
        public float sideBurstGap = 0.5f;
        public float sideYOffsetMin = -2f;
        public float sideYOffsetMax = 2f;
        public int sideMaxAlive = 12;

        [Header("Chase enemies")]
        public bool chaseEnabled = true;
        public Vector2 chaseBurstInterval = new Vector2(2.0f, 3.5f);
        public Vector2Int chaseBurstAmount = new Vector2Int(2, 6);
        public float chaseBurstGap = 0.5f;
        public int chaseMaxAlive = 10;

        [Header("Stationary enemies (spawn once at a kill threshold)")]
        public bool stationaryEnabled = false;
        public int stationaryAmount = 2;
        public int stationaryMaxAlive = 2;
        public int spawnStationaryAtKill = -1; // e.g. 10. set -1 to disable
    }

    [Header("Refs")]
    [SerializeField] private EnemySpawner spawner;

    [Header("6 waves total (midboss after Wave 3)")]
    [SerializeField] private List<WaveDef> waves = new List<WaveDef>(6);

    [Header("Bosses")]
    [SerializeField] private Primadon midBossPrefab;   // easy version
    [SerializeField] private Primadon finalBossPrefab; // hard version
    [SerializeField] private Transform bossSpawnPoint;

    [Header("Between phases")]
    [SerializeField] private float intermissionSeconds = 2.0f;
    [SerializeField] private bool waitForCleanupBeforeNextWave = true;

    private int killsThisWave = 0;
    private bool countingKills = false;
    private bool bossActive = false;

    private void Awake()
    {
        if (spawner == null) spawner = FindFirstObjectByType<EnemySpawner>();
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

        // don't count boss deaths toward wave clears
        if (e is Primadon) return;

        killsThisWave++;
    }

    private void Start()
    {
        StartCoroutine(RunLevel());
    }

    private IEnumerator RunLevel()
    {
        // Safety
        if (spawner == null || waves == null || waves.Count == 0)
            yield break;

        // ---- Waves 1-3 ----
        for (int i = 0; i < waves.Count && i < 3; i++)
        {
            yield return RunWave(waves[i]);
            yield return new WaitForSeconds(intermissionSeconds);
        }

        // ---- Midboss (easy) ----
        yield return RunBoss(midBossPrefab);
        yield return new WaitForSeconds(intermissionSeconds);

        // ---- Waves 4-6 ----
        for (int i = 3; i < waves.Count && i < 6; i++)
        {
            yield return RunWave(waves[i]);
            yield return new WaitForSeconds(intermissionSeconds);
        }

        // ---- Final boss (hard) ----
        yield return RunBoss(finalBossPrefab);

        // TODO: win screen / score result
    }

    private IEnumerator RunWave(WaveDef w)
    {
        killsThisWave = 0;
        countingKills = true;
        bossActive = false;

        float nextSide = Time.time + Random.Range(w.sideBurstInterval.x, w.sideBurstInterval.y);
        float nextChase = Time.time + Random.Range(w.chaseBurstInterval.x, w.chaseBurstInterval.y);

        bool spawnedStationary = false;

        while (killsThisWave < w.killsToClear)
        {
            if (spawner == null) yield break;

            spawner.CleanupNulls();

            // Stationary trigger (spawn ONCE at a kill count)
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

        if (!waitForCleanupBeforeNextWave) yield break;

        // Optional cleanup: wait until the screen is clear before moving on
        while (spawner.AliveSideCount > 0 || spawner.AliveChaseCount > 0 || spawner.AliveStationaryCount > 0)
            yield return null;
    }

    private IEnumerator RunBoss(Primadon bossPrefab)
    {
        if (bossPrefab == null || bossSpawnPoint == null)
            yield break;

        // Stop counting wave kills during boss
        countingKills = false;
        bossActive = true;

        // Wait for trash to clear (feels clean)
        if (waitForCleanupBeforeNextWave && spawner != null)
        {
            while (spawner.AliveSideCount > 0 || spawner.AliveChaseCount > 0 || spawner.AliveStationaryCount > 0)
                yield return null;
        }

        Primadon boss = Instantiate(bossPrefab, bossSpawnPoint.position, bossSpawnPoint.rotation);

        // Wait for boss death (destroy)
        while (boss != null)
            yield return null;

        bossActive = false;
    }
}
