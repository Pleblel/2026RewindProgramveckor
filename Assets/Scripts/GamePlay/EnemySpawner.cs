using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject stationaryEnemy;
    [SerializeField] GameObject chaseEnemy;

    [Header("Tracking")]
    [SerializeField] List<GameObject> stationaryEnemies = new List<GameObject>();
    [SerializeField] List<GameObject> chaseEnemies = new List<GameObject>();

    [Header("Stationary Spawning")]
    [SerializeField] Transform stationarySpawnPoint;
    [SerializeField] List<Transform> stationarySeatTargets = new List<Transform>();
    [SerializeField] int stationarySpawnAmount = 2;
    [SerializeField] int minStationaryRespawnTime = 4;
    [SerializeField] int maxStationaryRespawnTime = 9;

    [Header("Chase Spawning")]
    [SerializeField] List<Transform> chaseEnemySpawns = new List<Transform>();
    [SerializeField] int chaseSpawnAmount;         // enemies per burst
    [SerializeField] float chaseSpawnGap = 0.5f;       // seconds between each spawn in the burst
    [SerializeField] int minChaseRespawnTime = 3;      // time between bursts
    [SerializeField] int maxChaseRespawnTime = 5;

    private float stationaryTimer = 0f;
    private float chaseTimer = 0f;
    private bool spawningChase = false;

    void Start()
    {
        // Start with random initial delays (set to 0f if you want immediate spawns)
        stationaryTimer = Random.Range(minStationaryRespawnTime, maxStationaryRespawnTime + 1);
        chaseTimer = Random.Range(minChaseRespawnTime, maxChaseRespawnTime + 1);
    }

    void Update()
    {
        CleanupNulls();

        HandleStationarySpawns();
        HandleChaseSpawns();
    }

    // ---------------- Stationary ----------------

    void HandleStationarySpawns()
    {
        // If ANY stationary is alive, do nothing (no spawning + timer doesn't tick)
        if (stationaryEnemies.Count > 0)
            return;

        // None alive -> countdown
        if (stationaryTimer > 0f)
        {
            stationaryTimer -= Time.deltaTime;
            return;
        }

        // Timer done -> spawn wave, reset timer (next wave only after they die again)
        SpawnStationaryEnemies();
        stationaryTimer = Random.Range(minStationaryRespawnTime, maxStationaryRespawnTime + 1);
    }

    void SpawnStationaryEnemies()
    {
        if (stationaryEnemy == null || stationarySpawnPoint == null)
        {
            Debug.LogError("StationaryEnemy prefab or StationarySpawnPoint is not assigned.");
            return;
        }

        if (stationarySeatTargets == null || stationarySeatTargets.Count < stationarySpawnAmount)
        {
            Debug.LogError("Not enough stationarySeatTargets for stationarySpawnAmount.");
            return;
        }

        // Pick unique seats
        List<int> indices = new List<int>();
        for (int i = 0; i < stationarySeatTargets.Count; i++)
            indices.Add(i);

        for (int i = 0; i < stationarySpawnAmount; i++)
        {
            int pick = Random.Range(0, indices.Count);
            int seatIndex = indices[pick];
            indices.RemoveAt(pick);

            Transform seat = stationarySeatTargets[seatIndex];

            GameObject enemyGO = Instantiate(stationaryEnemy, stationarySpawnPoint.position, stationarySpawnPoint.rotation);
            stationaryEnemies.Add(enemyGO);

            var enemy = enemyGO.GetComponent<StationaryShootingEnemy>();
            if (enemy != null)
                enemy.FindTargetPosition(seat);
            else
                Debug.LogWarning("Spawned stationaryEnemy has no StationaryShootingEnemy component.");
        }
    }

    // ---------------- Chase ----------------

    void HandleChaseSpawns()
    {
        if (spawningChase)
            return;

        if (chaseEnemy == null || chaseEnemySpawns == null || chaseEnemySpawns.Count == 0)
            return;

        if (chaseTimer > 0f)
        {
            chaseTimer -= Time.deltaTime;
            return;
        }

        // Start burst and reset timer for next burst
        StartCoroutine(SpawnChaseBurst());
        chaseTimer = Random.Range(minChaseRespawnTime, maxChaseRespawnTime + 1);
    }

    IEnumerator SpawnChaseBurst()
    {
        spawningChase = true;

        chaseSpawnAmount = Random.Range(3, 13);

        for (int i = 0; i < chaseSpawnAmount; i++)
        {
            Transform sp = chaseEnemySpawns[Random.Range(0, chaseEnemySpawns.Count)];
            GameObject enemyGO = Instantiate(chaseEnemy, sp.position, sp.rotation);
            chaseEnemies.Add(enemyGO);

            if (i < chaseSpawnAmount - 1)
                yield return new WaitForSeconds(chaseSpawnGap);
        }

        spawningChase = false;
    }

    // ---------------- Cleanup ----------------

    void CleanupNulls()
    {
        stationaryEnemies.RemoveAll(e => e == null);
        chaseEnemies.RemoveAll(e => e == null);
    }
}
