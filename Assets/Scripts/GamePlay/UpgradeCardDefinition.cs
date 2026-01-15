using UnityEngine;

[System.Serializable]
public class UpgradeCardDefinition
{
    public UpgradeId id;
    public string title;
    [TextArea] public string description;

    [Range(0.01f, 10f)] public float weight = 1f;   // higher = appears more often
    public bool canRepeatInRun = true;
}
