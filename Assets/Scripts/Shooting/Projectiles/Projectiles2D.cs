using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile2D : MonoBehaviour
{
    public float speed = 14f;
    public float lifetime = 3f;

    private Rigidbody2D _rb;
    private float _t;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _rb.gravityScale = 0f;
    }

    public void Init(Vector2 dir, float overrideSpeed = -1f, float overrideLifetime = -1f)
    {
        if (overrideSpeed > 0f) speed = overrideSpeed;
        if (overrideLifetime > 0f) lifetime = overrideLifetime;

        dir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector2.right;
        _rb.velocity = dir * speed;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        _t = 0f;
    }

    private void Update()
    {
        _t += Time.deltaTime;
        if (_t >= lifetime) Destroy(gameObject);
    }
}