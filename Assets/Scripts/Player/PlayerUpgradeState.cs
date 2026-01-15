using UnityEngine;

public class PlayerUpgradeState : MonoBehaviour
{
    [Header("Core multipliers")]
    public float damageMult = 1f;
    public float fireRateMult = 1f;
    public float bulletSpeedMult = 1f;

    [Header("Default vs Focus")]
    public float defaultDamageMult = 1f;
    public float defaultFireRateMult = 1f;
    public float defaultSpreadMult = 1f;  // affects ONLY default spread (focus is always 0)

    public float focusedDamageMult = 1f;
    public float focusedFireRateMult = 1f;

    [Header("Formation")]
    public float tripleSpacingMult = 1f;

    [Header("Movement")]
    public float focusedMoveMult = 1f;

    [Header("Special")]
    public int pierceBonus = 0;

    [Header("Barrier / revive")]
    public bool waveBarrierEnabled = false;
    public int barrierPerWave = 0;
    public int barrierHits = 0;
    public int revives = 0;

    [Header("Kill Bloom")]
    public bool killBloomEnabled = false;

    [Header("Damage targeting")]
    public float bossAndStationaryDamageMult = 1f;
    public float trashDamageMult = 1f;

    [Header("Slow enemy bullets while focused")]
    public bool slowEnemyBulletsWhileFocused = false;
    public float focusedEnemyBulletSpeedMult = 0.90f;
}
