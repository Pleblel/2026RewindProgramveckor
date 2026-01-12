using UnityEngine;

public class ArcadeAim2D : MonoBehaviour
{
    public enum AimMode
    {
        FixedDirection,
        LastMoveDirection,
        SeparateAimInput
    }

    public enum QuantizeMode { None, FourWay, EightWay }

    [Header("Mode")]
    public AimMode aimMode = AimMode.LastMoveDirection;
    public QuantizeMode quantize = QuantizeMode.EightWay;

    [Header("Fixed Direction (if AimMode = FixedDirection)")]
    public Vector2 fixedDirection = Vector2.up;

    [Header("Input axes (old Input Manager)")]
    public string moveX = "Horizontal";
    public string moveY = "Vertical";

    public string aimX = "AimHorizontal";
    public string aimY = "AimVertical";

    [Header("Defaults")]
    public Vector2 defaultDirection = Vector2.right;

    private Vector2 _lastNonZero = Vector2.right;

    public Vector2 GetAimDirection()
    {
        Vector2 raw = defaultDirection;

        switch (aimMode)
        {
            case AimMode.FixedDirection:
                raw = fixedDirection;
                break;

            case AimMode.LastMoveDirection:
                raw = new Vector2(Input.GetAxisRaw(moveX), Input.GetAxisRaw(moveY));
                if (raw.sqrMagnitude > 0.001f) _lastNonZero = raw.normalized;
                raw = _lastNonZero;
                break;

            case AimMode.SeparateAimInput:
                raw = new Vector2(Input.GetAxisRaw(aimX), Input.GetAxisRaw(aimY));
                if (raw.sqrMagnitude > 0.001f) _lastNonZero = raw.normalized;
                raw = _lastNonZero;
                break;
        }

        if (raw.sqrMagnitude <= 0.0001f) raw = defaultDirection;
        raw = raw.normalized;

        return Quantize(raw, quantize);
    }

    private static Vector2 Quantize(Vector2 dir, QuantizeMode mode)
    {
        if (mode == QuantizeMode.None) return dir;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        float step = mode == QuantizeMode.FourWay ? 90f : 45f;
        float snapped = Mathf.Round(angle / step) * step;
        float rad = snapped * Mathf.Deg2Rad;
        return new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;
    }
}