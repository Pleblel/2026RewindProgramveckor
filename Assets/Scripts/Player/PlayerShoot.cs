using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    [Header("Bullets")]
    public BulletType[] bullets;
    [SerializeField] int selectedBulletIndex = 0;

    [Header("Base stats")]
    public float baseFireRate = 5f;
    public int baseDamagePerBullet = 10;

    [Header("Spread")]
    public float defaultSpreadDeg = 6f; // focus becomes 0 always

    [Header("Triple spacing")]
    public float baseTripleOffsetX = 0.2f;

    private Transform firePoint;
    private float cooldown;

    private PlayerMovement pm;
    private PlayerUpgradeState ups;

    private void Start()
    {
        firePoint = transform.Find("Fire Point");
        pm = GetComponent<PlayerMovement>();
        ups = GetComponent<PlayerUpgradeState>();
    }

    private bool IsFocused()
    {
        return pm != null && pm.speed == PlayerMovement.Speed.Focused;
    }

    private void Update()
    {
        // (optional bullet switching)
        for (int i = 0; i < bullets.Length && i < 9; i++)
        {
            KeyCode key = (KeyCode)((int)KeyCode.Alpha1 + i);
            if (Input.GetKeyDown(key)) selectedBulletIndex = i;
        }

        float fr = GetEffectiveFireRate();
        if (Input.GetKey(KeyCode.J) && Time.time >= cooldown)
        {
            cooldown = Time.time + (1f / Mathf.Max(0.01f, fr));
            ShootTriple();
        }
    }

    private float GetEffectiveFireRate()
    {
        float fr = baseFireRate;
        if (ups != null)
        {
            fr *= ups.fireRateMult;
            fr *= IsFocused() ? ups.focusedFireRateMult : ups.defaultFireRateMult;
        }
        return fr;
    }

    public int GetEffectiveDamage(EnemyEntity enemy)
    {
        float dmg = baseDamagePerBullet;

        if (ups != null)
        {
            dmg *= ups.damageMult;
            dmg *= IsFocused() ? ups.focusedDamageMult : ups.defaultDamageMult;

            if (enemy != null)
            {
                bool bossOrStationary = (enemy is Primadon) || (enemy is StationaryShootingEnemy);
                dmg *= bossOrStationary ? ups.bossAndStationaryDamageMult : ups.trashDamageMult;
            }
        }

        return Mathf.Max(1, Mathf.RoundToInt(dmg));
    }

    private float GetEffectiveBulletSpeed(float baseSpeed)
    {
        float s = baseSpeed;
        if (ups != null) s *= ups.bulletSpeedMult;
        return s;
    }

    private float GetSpreadDeg()
    {
        if (IsFocused()) return 0f;
        float s = defaultSpreadDeg;
        if (ups != null) s *= ups.defaultSpreadMult;
        return s;
    }

    private float GetTripleOffsetX()
    {
        float m = ups != null ? ups.tripleSpacingMult : 1f;
        return baseTripleOffsetX * m;
    }

    private Vector2 ApplySpread(Vector2 baseDir, float spreadDeg)
    {
        float half = spreadDeg * 0.5f;
        float angle = Random.Range(-half, half);
        return (Quaternion.Euler(0, 0, angle) * baseDir).normalized;
    }

    private void ShootTriple()
    {
        if (bullets == null || bullets.Length == 0) return;
        if (firePoint == null) return;

        selectedBulletIndex = Mathf.Clamp(selectedBulletIndex, 0, bullets.Length - 1);
        BulletType type = bullets[selectedBulletIndex];

        float spread = GetSpreadDeg();
        float off = GetTripleOffsetX();

        Vector2 baseDir = Vector2.up;

        Vector2 d0 = ApplySpread(baseDir, spread);
        Vector2 d1 = (Quaternion.Euler(0, 0, -spread / 2f) * baseDir).normalized;
        Vector2 d2 = (Quaternion.Euler(0, 0, spread / 2f) * baseDir).normalized;

        FireOne(type, d0, Vector2.zero);
        FireOne(type, d1, new Vector2(-off, 0));
        FireOne(type, d2, new Vector2(off, 0));
    }

    private void FireOne(BulletType type, Vector2 dir, Vector2 offset)
    {
        GameObject go = Instantiate(type.prefab, firePoint.position - (Vector3)offset, Quaternion.identity);
        go.transform.localScale = Vector3.one * type.scale;

        var pb = go.GetComponent<PlayerBulletSuperClass>();
        if (pb != null)
        {
            float speed = GetEffectiveBulletSpeed(type.speed);
            int pierce = ups != null ? ups.pierceBonus : 0;
            pb.Init(dir, speed, baseDamagePerBullet, pierce, this);
        }
    }

    // Used by Kill Bloom
    public void SpawnBloomBurst(Vector2 pos, int count, float speedMult, float damageMult)
    {
        if (bullets == null || bullets.Length == 0) return;

        selectedBulletIndex = Mathf.Clamp(selectedBulletIndex, 0, bullets.Length - 1);
        BulletType type = bullets[selectedBulletIndex];

        float step = 360f / Mathf.Max(1, count);
        for (int i = 0; i < count; i++)
        {
            float a = step * i;
            float rad = a * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            GameObject go = Instantiate(type.prefab, pos, Quaternion.identity);
            go.transform.localScale = Vector3.one * type.scale;

            var pb = go.GetComponent<PlayerBulletSuperClass>();
            if (pb != null)
            {
                float speed = GetEffectiveBulletSpeed(type.speed) * speedMult;
                int dmg = Mathf.Max(1, Mathf.RoundToInt(baseDamagePerBullet * damageMult));
                int pierce = ups != null ? ups.pierceBonus : 0;
                pb.Init(dir, speed, dmg, pierce, this);
            }
        }
    }
}

[System.Serializable]
public struct BulletType
{
    public string name;
    public float scale;
    public float speed;
    public GameObject prefab;
}
