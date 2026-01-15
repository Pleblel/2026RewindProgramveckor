using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float normalSpeed = 6f;
    [SerializeField] private float focusedDiv = 2f;

    public enum Speed { Default, Focused }
    public Speed speed { get; private set; } = Speed.Default;

    private Rigidbody2D rb;
    private Vector2 input;
    private float currentSpeed;

    private PlayerUpgradeState ups;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
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
        rb.MovePosition(rb.position + input * currentSpeed * Time.fixedDeltaTime);
    }
}
