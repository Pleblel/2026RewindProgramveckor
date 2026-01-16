using UnityEngine;
using UnityEngine.UI;

public class PrimadonHealthbarUI : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image barImage;
    [SerializeField] private CanvasGroup group; // optional (nice for hide/show)

    [Header("Frames (24 sprites)")]
    [Tooltip("Order: EMPTY -> FULL")]
    [SerializeField] private Sprite[] frames = new Sprite[24];

    [Header("Boss")]
    [SerializeField] private Primadon boss;                 // can be set by WaveManager
    [SerializeField] private bool autoFindBoss = true;
    [SerializeField] private float findEverySeconds = 0.5f;

    private float findTimer;

    private void Awake()
    {
        if (barImage == null) barImage = GetComponent<Image>();
        if (group == null) group = GetComponent<CanvasGroup>();

        SetVisible(false);
    }

    private void Update()
    {
        // Find boss when needed (midboss + final boss)
        if (boss == null && autoFindBoss)
        {
            findTimer -= Time.unscaledDeltaTime;
            if (findTimer <= 0f)
            {
                findTimer = findEverySeconds;
                boss = FindFirstObjectByType<Primadon>();
            }
        }

        // Hide bar unless boss exists
        if (boss == null)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        if (barImage == null || frames == null || frames.Length == 0)
            return;

        // Prefer public getters if you have them:
        // float cur = boss.CurrentHP;
        // float max = boss.MaxHP;

        float cur = boss.currentHP;
        float max = boss.enemyHP;

        float t = (max <= 0f) ? 0f : Mathf.Clamp01(cur / max);

        // EMPTY -> FULL mapping
        int last = frames.Length - 1;
        int index = Mathf.RoundToInt(t * last);
        index = Mathf.Clamp(index, 0, last);

        barImage.sprite = frames[index];
    }

    private void SetVisible(bool on)
    {
        if (barImage != null) barImage.enabled = on;

        // Optional nicer hide/show if CanvasGroup exists
        if (group != null)
        {
            group.alpha = on ? 1f : 0f;
            group.blocksRaycasts = on;
            group.interactable = on;
        }
    }

    // Call this when you spawn the boss (recommended)
    public void SetBoss(Primadon p)
    {
        boss = p;
        SetVisible(boss != null);
    }

    // Call this when boss dies (optional)
    public void ClearBoss()
    {
        boss = null;
        SetVisible(false);
    }
}
