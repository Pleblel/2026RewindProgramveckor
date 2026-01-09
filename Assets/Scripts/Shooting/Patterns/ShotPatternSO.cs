using System.Collections;
using UnityEngine;

public abstract class ShotPatternSO : ScriptableObject
{
    public abstract IEnumerator Fire(Gun2D gun, ShotContext ctx);
}