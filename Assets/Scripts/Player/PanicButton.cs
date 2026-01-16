using UnityEngine;

public class PanicBomb : MonoBehaviour
{
    [Header("Cost / Input")]
    [SerializeField] private KeyCode key = KeyCode.K;
    [SerializeField] private int scoreCost = 1_000_000;
    [SerializeField] private float cooldownSeconds = 6f;

    [Header("Effect")]
    [SerializeField] private bool clearAllEnemyBullets = true;

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

        if (cameraShake == null)
            cameraShake = Camera.main != null ? Camera.main.GetComponent<CameraShake>() : null;
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
        // Screen shake (unscaled so it works even if you pause elsewhere)
        if (cameraShake != null)
            cameraShake.Shake(shakeDuration, shakeStrength);

        // Clear all enemy bullets
        if (!clearAllEnemyBullets) return;

        var bullets = FindObjectsByType<EnemyBulletSuperClass>(FindObjectsSortMode.None);

        for (int i = 0; i < bullets.Length; i++)
        {
            if (bullets[i] != null)
                Destroy(bullets[i].gameObject);
        }
    }
}
