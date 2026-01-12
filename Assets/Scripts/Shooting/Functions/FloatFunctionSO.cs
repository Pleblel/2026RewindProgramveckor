using UnityEngine;

public abstract class FloatFunctionSO : ScriptableObject
{
    public abstract float Evaluate(in ShotContext ctx, float bulletIndex01);
}