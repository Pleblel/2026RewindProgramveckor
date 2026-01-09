using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using static PlayerMovement;

public class PlayerMovement : MonoBehaviour
{
    [Header("speed")]
    public float normalSpeed = 6f;
    private float currentSpeed;
    public enum Speed
    {
        Default,
        Unfocused,
        Focused
    }
    public Speed speed { get; private set; } = Speed.Default;

    [Header("Playfield margins")]
    [Range(0f, 0.49f)] public float leftMarginV = 0.20f;
    [Range(0f, 0.49f)] public float rightMarginV = 0.20f;
    [Range(0f, 0.49f)] public float bottomMarginV = 0.01f;
    [Range(0f, 0.49f)] public float topMarginV = 0.01f;

    private Rigidbody2D rb;
    private Vector2 input;
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
        bool unfocused = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool focused = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);

        if (focused) speed = Speed.Focused;
        else if (unfocused) speed = Speed.Unfocused;
        else speed = Speed.Default;


        switch (speed)
        {
            case Speed.Default:
                currentSpeed = normalSpeed;
                break;
            case Speed.Unfocused:
                currentSpeed = normalSpeed * 1.5f;
                break;

            case Speed.Focused:
                currentSpeed = normalSpeed / 2;
                break;
        }

    }

    void FixedUpdate()
    {
        Vector2 nextPos = rb.position + input * currentSpeed * Time.fixedDeltaTime;

        if (cam != null)
        {
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
