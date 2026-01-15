using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speed")]
    public float normalSpeed = 6f;
    [SerializeField] private float focusedDiv = 2f;

    public enum Speed { Default, Focused }
    public Speed speed { get; private set; } = Speed.Default;

    [Header("Viewport margins (0..0.49)")]
    [Range(0f, 0.49f)] public float leftMarginV = 0.20f;
    [Range(0f, 0.49f)] public float rightMarginV = 0.20f;
    [Range(0f, 0.49f)] public float bottomMarginV = 0.01f;
    [Range(0f, 0.49f)] public float topMarginV = 0.01f;

    private Rigidbody2D rb;
    private Vector2 input;
    private float currentSpeed;
    private Camera cam;

    private PlayerUpgradeState ups;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        cam = Camera.main;
        ups = GetComponent<PlayerUpgradeState>();
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        bool focused = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        speed = focused ? Speed.Focused : Speed.Default;

        float focusMult = ups != null ? ups.focusedMoveMult : 1f;

        currentSpeed = (speed == Speed.Focused)
            ? (normalSpeed / focusedDiv) * focusMult
            : normalSpeed;
    }

    void FixedUpdate()
    {
        Vector2 nextPos = rb.position + input * currentSpeed * Time.fixedDeltaTime;

        if (cam != null)
        {
            // Use z-distance to get correct world positions from viewport
            float zDist = Mathf.Abs(transform.position.z - cam.transform.position.z);

            float minX = cam.ViewportToWorldPoint(new Vector3(leftMarginV, 0f, zDist)).x;
            float maxX = cam.ViewportToWorldPoint(new Vector3(1f - rightMarginV, 0f, zDist)).x;

            float minY = cam.ViewportToWorldPoint(new Vector3(0f, bottomMarginV, zDist)).y;
            float maxY = cam.ViewportToWorldPoint(new Vector3(0f, 1f - topMarginV, zDist)).y;

            nextPos.x = Mathf.Clamp(nextPos.x, minX, maxX);
            nextPos.y = Mathf.Clamp(nextPos.y, minY, maxY);
        }

        rb.MovePosition(nextPos);
    }
}
