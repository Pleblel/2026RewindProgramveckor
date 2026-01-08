using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Speeds")]
    public float normalSpeed = 6f;
    public float focusSpeed = 3f;

    [Header("Playfield margins (viewport 0..1)")]
    [Tooltip("0.15 = 15% of the screen blocked on the left")]
    [Range(0f, 0.49f)] public float leftMarginV = 0.20f;
    [Range(0f, 0.49f)] public float rightMarginV = 0.20f;
    [Range(0f, 0.49f)] public float bottomMarginV = 0.01f;
    [Range(0f, 0.49f)] public float topMarginV = 0.01f;

    private Rigidbody2D rb;
    private Vector2 input;
    private bool focusing;
    private Camera cam;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
        cam = Camera.main;
    }

    void Update()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;
        focusing = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
    }

    void FixedUpdate()
    {
        float speed = focusing ? focusSpeed : normalSpeed;
        Vector2 nextPos = rb.position + input * speed * Time.fixedDeltaTime;

        if (cam != null)
        {
            // For orthographic camera z doesn't matter; for perspective we use distance to the player plane.
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
