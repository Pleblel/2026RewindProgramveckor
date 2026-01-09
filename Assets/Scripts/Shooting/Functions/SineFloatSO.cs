using UnityEngine;

[CreateAssetMenu(menuName = "Shooting/Functions/Sine Float")]
public class SineFloatSO : FloatFunctionSO
{
    public float amplitude = 20f;
    public float frequency = 2f;
    public float phaseDegrees = 0f;

    public bool useTime = true;
    public float bulletPhase = 0f;

    public override float Evaluate(in ShotContext ctx, float bulletIndex01)
    {
        float x = useTime ? ctx.time : ctx.shotIndex;
        float phase = (phaseDegrees * Mathf.Deg2Rad) + (bulletIndex01 * bulletPhase);
        return amplitude * Mathf.Sin((x * frequency * Mathf.PI * 2f) + phase);
    }
}