using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerSurvivability : MonoBehaviour
{
    [SerializeField] private float reviveInvulnTime = 1.5f;

    private Collider2D col;
    private SpriteRenderer sr;
    private PlayerUpgradeState ups;
    private Score score;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        sr = GetComponentInChildren<SpriteRenderer>();
        ups = GetComponent<PlayerUpgradeState>();
        score = GameObject.Find("GameManager")?.GetComponent<Score>();
    }

    // Call this at the start of each wave
    public void RefreshWaveBarrier()
    {
        if (ups == null) return;
        if (!ups.waveBarrierEnabled) return;

        ups.barrierHits = Mathf.Max(ups.barrierHits, ups.barrierPerWave);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("Hit: " + collision.name + " tag=" + collision.tag);
        // Tag your enemy bullets as "EnemyBullet" if you can.
        bool lethal = collision.CompareTag("EnemyBullet") || collision.CompareTag("Enemy");
        if (!lethal) return;

        if (ups != null && ups.barrierHits > 0)
        {
            ups.barrierHits--;
            Destroy(collision.gameObject);
            return;
        }

        if (ups != null && ups.revives > 0)
        {
            ups.revives--;
            Destroy(collision.gameObject);

            if (score != null)
                score.scoreMultiplier = Mathf.Max(0.1f, score.scoreMultiplier - 0.3f);

            StartCoroutine(InvulnFlash());
            return;
        }

        // Replace with your death handler if you have one
        Destroy(gameObject);
        SceneManager.LoadScene(1);
    }

    private IEnumerator InvulnFlash()
    {
        if (col != null) col.enabled = false;

        float end = Time.unscaledTime + reviveInvulnTime;
        while (Time.unscaledTime < end)
        {
            if (sr != null) sr.enabled = !sr.enabled;
            yield return new WaitForSecondsRealtime(0.08f);
        }

        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }
}
