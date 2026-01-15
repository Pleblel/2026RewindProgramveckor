using UnityEngine;

public class VerticalLoopAutoClone : MonoBehaviour
{
    [Header("Scroll")]
    [SerializeField] private float speed = 2f;

    [Header("Wrap")]
    [SerializeField] private float wrapBelowY = -12f;

    private Transform a;
    private Transform b;
    private float height;

    // marker so we don't clone twice
    private bool initialized = false;

    private void Awake()
    {
        // Prevent double-init (can happen if script gets duplicated / enabled again)
        if (initialized) return;
        initialized = true;

        var sr = GetComponent<SpriteRenderer>();
        if (sr == null)
        {
            Debug.LogError("VerticalLoopAutoClone needs a SpriteRenderer on the same GameObject.");
            enabled = false;
            return;
        }

        height = sr.bounds.size.y;
        a = transform;

        // If we already have a child named "BG_Clone", don't make another
        Transform existing = transform.parent != null ? transform.parent.Find(gameObject.name + "_Clone") : null;
        if (existing != null)
        {
            b = existing;
            return;
        }

        // Clone once
        GameObject clone = Instantiate(gameObject, transform.parent);
        clone.name = gameObject.name + "_Clone";
        b = clone.transform;

        // Remove THIS script from the clone so it never clones again
        var cloneScript = clone.GetComponent<VerticalLoopAutoClone>();
        if (cloneScript != null) Destroy(cloneScript);

        // Put clone above original
        b.position = new Vector3(a.position.x, a.position.y + height, a.position.z);
    }

    private void Update()
    {
        if (b == null) return;

        float dy = speed * Time.deltaTime;

        a.position += Vector3.down * dy;
        b.position += Vector3.down * dy;

        if (a.position.y <= wrapBelowY)
            a.position = new Vector3(a.position.x, b.position.y + height, a.position.z);

        if (b.position.y <= wrapBelowY)
            b.position = new Vector3(b.position.x, a.position.y + height, b.position.z);
    }
}
