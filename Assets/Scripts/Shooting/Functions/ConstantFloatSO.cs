using UnityEngine;

[CreateAssetMenu(menuName = "Shooting/Functions/Constant Float")]
public class ConstantFloatSO : FloatFunctionSO
{
    public float value;
    public override float Evaluate(in ShotContext ctx, float bulletIndex01) => value;
}