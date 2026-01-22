using System.Collections.Generic;
using UnityEngine;

public class CardRewardManager : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject cardPanel;
    [SerializeField] private List<CardButtonUI> cardButtons = new List<CardButtonUI>(5);

    [Header("Cards (leave empty to auto-fill 24)")]
    [SerializeField] private List<UpgradeCardDefinition> cards = new List<UpgradeCardDefinition>();

    private HashSet<UpgradeId> takenThisRun = new HashSet<UpgradeId>();
    private bool choosing = false;

    private PlayerUpgradeState ups;
    private PlayerSurvivability surv;
    private PlayerShoot shoot;
    private Score score;

    private void Awake()
    {
        var player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            ups = player.GetComponent<PlayerUpgradeState>() ?? player.AddComponent<PlayerUpgradeState>();
            surv = player.GetComponent<PlayerSurvivability>() ?? player.AddComponent<PlayerSurvivability>();
            shoot = player.GetComponent<PlayerShoot>();
        }

        score = GetComponent<Score>();

        if (cards == null || cards.Count == 0)
            cards = BuildDefault24();

        if (cardPanel != null)
            cardPanel.SetActive(false);

        // Kill Bloom hook (requires EnemyEntity.OnAnyEnemyKilled)
        EnemyEntity.OnAnyEnemyKilled += OnEnemyKilled;
    }


    private void Update()
    {
        if (choosing)
            Time.timeScale = 0f;
    }

    private void OnDestroy()
    {
        EnemyEntity.OnAnyEnemyKilled -= OnEnemyKilled;
    }

    private void OnEnemyKilled(EnemyEntity e)
    {
        if (ups == null || shoot == null) return;
        if (!ups.killBloomEnabled) return;
        if (e == null) return;

        // small 8-bullet ring (low damage feel)
        shoot.SpawnBloomBurst(e.transform.position, 8, 1.0f, 0.35f);
    }

    // Call at the START of each wave
    public void OnWaveStart()
    {
        if (surv != null)
            surv.RefreshWaveBarrier();
    }

    // Call at the END of each wave
    public void OpenCardChoice()
    {
        if (choosing) return;
        choosing = true;

        if (cardPanel != null)
            cardPanel.SetActive(true);

        Time.timeScale = 0f;

        var rolled = RollUnique(5);
        for (int i = 0; i < cardButtons.Count && i < rolled.Count; i++)
            cardButtons[i].Setup(this, rolled[i]);
    }

    public void PickCard(UpgradeId id)
    {
        ApplyUpgrade(id);

        if (cardPanel != null)
            cardPanel.SetActive(false);

        Time.timeScale = 1f;
        choosing = false;
    }

    // ---------- APPLY ----------
    private void ApplyUpgrade(UpgradeId id)
    {
        takenThisRun.Add(id);
        if (ups == null) return;

        switch (id)
        {
            // Common (14)
            case UpgradeId.PowerI: ups.damageMult *= 1.10f; break;
            case UpgradeId.PowerII: ups.damageMult *= 1.15f; break;

            case UpgradeId.RapidI: ups.fireRateMult *= 1.10f; break;
            case UpgradeId.RapidII: ups.fireRateMult *= 1.15f; break;

            case UpgradeId.VelocityI: ups.bulletSpeedMult *= 1.15f; break;
            case UpgradeId.VelocityII:
                ups.bulletSpeedMult *= 1.25f;
                ups.damageMult *= 0.95f;
                break;

            case UpgradeId.TightFormation: ups.tripleSpacingMult *= 0.75f; break;
            case UpgradeId.WideFormation: ups.tripleSpacingMult *= 1.25f; break;

            case UpgradeId.StableHands: ups.defaultSpreadMult *= 0.80f; break;

            case UpgradeId.SnapFocus: ups.focusedMoveMult *= 1.25f; break;

            case UpgradeId.FocusPrecision: ups.focusedDamageMult *= 1.12f; break;
            case UpgradeId.FocusTempo: ups.focusedFireRateMult *= 1.12f; break;

            case UpgradeId.WaveBarrier:
                ups.waveBarrierEnabled = true;
                ups.barrierPerWave = Mathf.Max(ups.barrierPerWave, 1);
                break;

            case UpgradeId.SecondChance:
                ups.revives += 1;
                break;

            // Rare (6)
            case UpgradeId.FocusedCannon:
                ups.focusedDamageMult *= 1.25f;
                ups.defaultFireRateMult *= 0.90f;
                break;

            case UpgradeId.DefaultOverdrive:
                ups.defaultFireRateMult *= 1.20f;
                ups.focusedMoveMult *= 0.90f;
                break;

            case UpgradeId.PierceRounds:
                ups.pierceBonus += 1;
                ups.damageMult *= 0.92f;
                break;

            case UpgradeId.ExecutionProtocol:
                ups.bossAndStationaryDamageMult *= 1.30f;
                ups.trashDamageMult *= 0.90f;
                break;

            case UpgradeId.KillBloom:
                ups.killBloomEnabled = true;
                break;

            case UpgradeId.SlowTimeFocus:
                ups.slowEnemyBulletsWhileFocused = true;
                ups.focusedEnemyBulletSpeedMult = 0.90f;
                break;

            // Cursed (4)
            case UpgradeId.RankUpVelocity:
                if (score != null) score.scoreMultiplier += 0.5f;
                DifficultyRankBulletSpeed(0.12f);
                break;

            case UpgradeId.RankUpDensity:
                if (score != null) score.scoreMultiplier += 0.5f;
                DifficultyRankFireRate(0.15f);
                break;

            case UpgradeId.RankUpSwarm:
                if (score != null) score.scoreMultiplier += 0.5f;
                DifficultyRankSpawn(0.15f);
                break;

            case UpgradeId.NoRest:
                if (score != null) score.scoreMultiplier += 0.7f;
                DifficultyRankSpawn(0.10f);
                DifficultyRankFireRate(0.10f);
                break;
        }
    }

    // Optional hooks (won’t crash if you don’t have DifficultyManager)
    private void DifficultyRankBulletSpeed(float add)
    {
        var dm = FindFirstObjectByType<DifficultyManager>();
        if (dm != null) dm.AddRankBulletSpeed(add);
    }

    private void DifficultyRankFireRate(float add)
    {
        var dm = FindFirstObjectByType<DifficultyManager>();
        if (dm != null) dm.AddRankFireRate(add);
    }

    private void DifficultyRankSpawn(float add)
    {
        var dm = FindFirstObjectByType<DifficultyManager>();
        if (dm != null) dm.AddRankSpawn(add);
    }

    // ---------- ROLL ----------
    private List<UpgradeCardDefinition> RollUnique(int count)
    {
        List<UpgradeCardDefinition> pool = new List<UpgradeCardDefinition>();
        foreach (var c in cards)
        {
            if (c == null) continue;
            if (!c.canRepeatInRun && takenThisRun.Contains(c.id)) continue;
            pool.Add(c);
        }

        List<UpgradeCardDefinition> result = new List<UpgradeCardDefinition>(count);
        HashSet<UpgradeId> usedThisRoll = new HashSet<UpgradeId>();

        for (int i = 0; i < count && pool.Count > 0; i++)
        {
            var pick = WeightedPick(pool, usedThisRoll);
            if (pick == null) break;

            result.Add(pick);
            usedThisRoll.Add(pick.id);
        }

        // fill any shortfall
        if (result.Count < count)
        {
            foreach (var c in pool)
            {
                if (result.Count >= count) break;
                if (usedThisRoll.Contains(c.id)) continue;
                result.Add(c);
                usedThisRoll.Add(c.id);
            }
        }

        return result;
    }

    private UpgradeCardDefinition WeightedPick(List<UpgradeCardDefinition> pool, HashSet<UpgradeId> used)
    {
        float total = 0f;
        foreach (var c in pool)
        {
            if (used.Contains(c.id)) continue;
            total += Mathf.Max(0.01f, c.weight);
        }

        float r = Random.Range(0f, total);
        float acc = 0f;

        foreach (var c in pool)
        {
            if (used.Contains(c.id)) continue;

            acc += Mathf.Max(0.01f, c.weight);
            if (r <= acc) return c;
        }

        return null;
    }

    // ---------- DEFAULT 24 ----------
    private List<UpgradeCardDefinition> BuildDefault24()
    {
        float C = 1.0f, R = 0.5f, X = 0.25f;

        return new List<UpgradeCardDefinition>
        {
            // Common (14)
            Card(UpgradeId.PowerI, "Power I", "Damage +10%.", C, true),
            Card(UpgradeId.PowerII, "Power II", "Damage +15%.", C, true),
            Card(UpgradeId.RapidI, "Rapid I", "Fire rate +10%.", C, true),
            Card(UpgradeId.RapidII, "Rapid II", "Fire rate +15%.", C, true),
            Card(UpgradeId.VelocityI, "Velocity I", "Bullet speed +15%.", C, true),
            Card(UpgradeId.VelocityII, "Velocity II", "Bullet speed +25%, Damage -5%.", C, true),
            Card(UpgradeId.TightFormation, "Tight Formation", "Triple spacing -25%.", C, true),
            Card(UpgradeId.WideFormation, "Wide Formation", "Triple spacing +25%.", C, true),
            Card(UpgradeId.StableHands, "Stable Hands", "Default spread -20%.", C, true),
            Card(UpgradeId.SnapFocus, "Snap Focus", "Focused move speed +25%.", C, true),
            Card(UpgradeId.FocusPrecision, "Focus Precision", "While focused: Damage +12%.", C, true),
            Card(UpgradeId.FocusTempo, "Focus Tempo", "While focused: Fire rate +12%.", C, true),
            Card(UpgradeId.WaveBarrier, "Wave Barrier", "Start each wave with 1 barrier hit.", C, false),
            Card(UpgradeId.SecondChance, "Second Chance", "Gain 1 revive. On revive: score multiplier -0.3.", C, false),

            // Rare (6)
            Card(UpgradeId.FocusedCannon, "Focused Cannon", "Focused damage +25%. Default fire rate -10%.", R, true),
            Card(UpgradeId.DefaultOverdrive, "Default Overdrive", "Default fire rate +20%. Focus move speed -10%.", R, true),
            Card(UpgradeId.PierceRounds, "Pierce Rounds", "Bullets pierce +1 enemy. Damage -8%.", R, true),
            Card(UpgradeId.ExecutionProtocol, "Execution Protocol", "Boss+Stationary: +30% dmg. Others: -10% dmg.", R, true),
            Card(UpgradeId.KillBloom, "Kill Bloom", "On kill: small 8-bullet ring.", R, false),
            Card(UpgradeId.SlowTimeFocus, "Slow Time Focus", "While focused: enemy bullets slower.", R, false),

            // Cursed (4)
            Card(UpgradeId.RankUpVelocity, "Rank Up: Velocity", "Score +0.5. Enemy bullet speed +12%.", X, true),
            Card(UpgradeId.RankUpDensity, "Rank Up: Density", "Score +0.5. Enemies fire +15% faster.", X, true),
            Card(UpgradeId.RankUpSwarm, "Rank Up: Swarm", "Score +0.5. More enemies spawn.", X, true),
            Card(UpgradeId.NoRest, "No Rest", "Score +0.7. Faster spawns + faster enemy fire.", X, true),
        };
    }

    private UpgradeCardDefinition Card(UpgradeId id, string title, string desc, float weight, bool canRepeat)
    {
        return new UpgradeCardDefinition
        {
            id = id,
            title = title,
            description = desc,
            weight = weight,
            canRepeatInRun = canRepeat
        };
    }
}
