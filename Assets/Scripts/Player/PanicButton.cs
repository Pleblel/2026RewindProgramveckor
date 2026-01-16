using UnityEngine;

public class PanicBomb : MonoBehaviour
{
    [Header("Input / Cost")]
    [SerializeField] private KeyCode key = KeyCode.K;
    [SerializeField] private int scoreCost = 1_000_000;
    [SerializeField] private float cooldownSeconds = 6f;

    [Header("Bullet Clear")]
    [SerializeField] private string enemyBulletTag = "EnemyBullet";

    [Header("Enemy Damage")]
    [SerializeField] private bool damageEnemies = true;
    [SerializeField] private float damageToAllEnemies = 50f;
    [SerializeField] private float damageToBosses = 200f;

    [Header("VFX")]
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private float shakeDuration = 0.14f;
    [SerializeField] private float shakeStrength = 0.22f;

    [Header("Refs")]
    [SerializeField] private Score score;

    private float nextUseTime;

    private void Awake()
    {
        if (score == null)
            score = GameObject.Find("GameManager")?.GetComponent<Score>();

        if (cameraShake == null && Camera.main != null)
            cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    private void Update()
    {
        if (!Input.GetKeyDown(key)) return;
        if (Time.time < nextUseTime) return;
        if (score == null) return;

        if (!score.TrySpendScore(scoreCost))
            return;

        nextUseTime = Time.time + cooldownSeconds;

        DoPanic();
    }

    private void DoPanic()
    {
        // VFX
        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeStrength);

        // 1) Clear all bullets with tag
        if (!string.IsNullOrEmpty(enemyBulletTag))
        {
            GameObject[] bullets = GameObject.FindGameObjectsWithTag(enemyBulletTag);
            for (int i = 0; i < bullets.Length; i++)
            {
                if (bullets[i] != null)
                    Destroy(bullets[i]);
            }
        }

        // 2) Damage enemies (still by component)
        if (damageEnemies)
        {

            var enemies = FindObjectsByType<EnemyEntity>(FindObjectsSortMode.None);

            for (int i = 0; i < enemies.Length; i++)
            {
                var e = enemies[i];
                if (e == null) continue;

                float dmg = (e is Primadon) ? damageToBosses : damageToAllEnemies;
                e.TakeDamage(dmg);
            }
        }
    }
}
