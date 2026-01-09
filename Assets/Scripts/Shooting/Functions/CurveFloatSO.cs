using UnityEngine;

[CreateAssetMenu(menuName = "Shooting/Functions/Curve Float")]
public class CurveFloatSO : FloatFunctionSO
{
    public AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
    public float multiplier = 1f;

    public override float Evaluate(in ShotContext ctx, float bulletIndex01)
    {
        return curve.Evaluate(Mathf.Clamp01(bulletIndex01)) * multiplier;
    }
}