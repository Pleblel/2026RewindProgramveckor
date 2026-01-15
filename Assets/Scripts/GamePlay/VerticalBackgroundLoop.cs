using UnityEngine;

public class VerticalBackgroundLoopManager : MonoBehaviour
{
    [Header("Background pieces (two copies of same sprite)")]
    [SerializeField] private Transform bgA;
    [SerializeField] private Transform bgB;

    [Header("Scroll")]
    [SerializeField] private float speed = 2f; // units/sec downward

    [Header("Wrap")]
    [Tooltip("When a piece goes below this world Y, it wraps above the other.")]
    [SerializeField] private float wrapBelowY = -12f;

    private float height;

    private void Awake()
    {
        if (bgA == null || bgB == null)
        {
            Debug.LogError("Assign bgA and bgB in the inspector.");
            enabled = false;
            return;
        }

        var srA = bgA.GetComponent<SpriteRenderer>();
        if (srA == null)
        {
            Debug.LogError("bgA must have a SpriteRenderer.");
            enabled = false;
            return;
        }

        // Auto-calc sprite height in world units
        height = srA.bounds.size.y;

        // Force bgB to be exactly above bgA so there is no seam
        bgB.position = new Vector3(bgB.position.x, bgA.position.y + height, bgB.position.z);
    }

    private void Update()
    {
        float dy = speed * Time.deltaTime;

        bgA.position += Vector3.down * dy;
        bgB.position += Vector3.down * dy;

        // Wrap whichever piece went below the wrap line
        if (bgA.position.y <= wrapBelowY)
            bgA.position = new Vector3(bgA.position.x, bgB.position.y + height, bgA.position.z);

        if (bgB.position.y <= wrapBelowY)
            bgB.position = new Vector3(bgB.position.x, bgA.position.y + height, bgB.position.z);
    }
}
