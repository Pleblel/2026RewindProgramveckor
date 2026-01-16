using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float defaultDuration = 0.12f;
    [SerializeField] private float defaultStrength = 0.18f;

    private Vector3 startLocalPos;
    private Coroutine routine;

    private void Awake()
    {
        startLocalPos = transform.localPosition;
    }

    public void Shake(float duration = -1f, float strength = -1f)
    {
        if (duration <= 0f) duration = defaultDuration;
        if (strength <= 0f) strength = defaultStrength;

        if (routine != null) StopCoroutine(routine);
        routine = StartCoroutine(ShakeRoutine(duration, strength));
    }

    private IEnumerator ShakeRoutine(float duration, float strength)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            Vector2 off = Random.insideUnitCircle * strength;
            transform.localPosition = startLocalPos + new Vector3(off.x, off.y, 0f);
            yield return null;
        }

        transform.localPosition = startLocalPos;
        routine = null;
    }
}
