using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] GameObject stationaryEnemy;
    [SerializeField] GameObject chaseEnemy;
    [SerializeField] GameObject sideEnemy;

    [Header("Tracking")]
    [SerializeField] List<GameObject> stationaryEnemies = new List<GameObject>();
    [SerializeField] List<GameObject> chaseEnemies = new List<GameObject>();
    [SerializeField] List<GameObject> sideEnemies = new List<GameObject>();

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
    [SerializeField] int minChaseRespawnTime = 3;         // time between bursts
    [SerializeField] int maxChaseRespawnTime = 5;

    [Header("SideScroll Spawning")]
    [SerializeField] List<Transform> sideEnemySpawns = new List<Transform>();
    [SerializeField] int sideSpawnAmount;         // enemies per burst
    [SerializeField] float sideSpawnGap = 0.5f;       // seconds between each spawn in the burst
    [SerializeField] int minSideRespawnTime = 3;         // time between bursts
    [SerializeField] int maxSideRespawnTime = 5;


    private float stationaryTimer = 0f;
    private float chaseTimer = 0f;
    private float sideTimer = 0f;
    private int sideSpawnOffset = 0;
    private bool spawningChase = false;
    private bool spawningSide = false;

    void Start()
    {
        // Start with random initial delays (set to 0f if you want immediate spawns)
        stationaryTimer = Random.Range(minStationaryRespawnTime, maxStationaryRespawnTime + 1);
        chaseTimer = Random.Range(minChaseRespawnTime, maxChaseRespawnTime + 1);
        sideTimer = Random.Range(minSideRespawnTime, maxSideRespawnTime + 1);
    }

    void Update()
    {
        CleanupNulls();

        HandleStationarySpawns();
        HandleChaseSpawns();
        HandleSideSpawns();
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

    // ---------------- Side Enemy --------------

    void HandleSideSpawns()
    {
        if (spawningSide)
            return;

        if (sideEnemy == null || sideEnemySpawns == null || sideEnemySpawns.Count == 0)
            return;

        if (sideTimer > 0f)
        {
            sideTimer -= Time.deltaTime;
            return;
        }

        // Start burst and reset timer for next burst
        StartCoroutine(SpawnSideBurst());
        sideTimer = Random.Range(minSideRespawnTime, maxSideRespawnTime + 1);
    }


    IEnumerator SpawnSideBurst()
    {
        spawningSide = true;

        sideSpawnAmount = Random.Range(3, 13);
        

        for (int i = 0; i < sideSpawnAmount; i++)
        {
            sideSpawnOffset = Random.Range(-1, 2);
            Transform sp = sideEnemySpawns[Random.Range(0, chaseEnemySpawns.Count)];
            Vector3 pos =  sp.position + new Vector3(0f, sideSpawnOffset, 0f);
            GameObject enemyGO = Instantiate(sideEnemy,  pos, sp.rotation);
            sideEnemies.Add(enemyGO);

            if (i < sideSpawnAmount - 1)
                yield return new WaitForSeconds(sideSpawnGap);
        }

        spawningSide = false;
    }


    // ---------------- Cleanup ----------------

    void CleanupNulls()
    {
        stationaryEnemies.RemoveAll(e => e == null);
        chaseEnemies.RemoveAll(e => e == null);
    }
}
