using Unity.Mathematics;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public enum ShootPattern { Single, Double, Tripple, Tracking }
    public ShootPattern pattern;

    [Header("Bullets")]
    public BulletType[] bullets;
    [SerializeField] int selectedBulletIndex = 0;
    [SerializeField] float fireRate = 5f;

    [Header("Spread")]
    [SerializeField] float focusedSpreadDeg;
    [SerializeField] float unfocusedSpreadDeg;
    [SerializeField] float defaultSpreadDeg;


    Transform firePoint;
    float cooldown;

    float spreadDeg;

    PlayerMovement playerMovement;

    private void Start()
    {
        firePoint = GetComponentInChildren<Transform>().Find("Fire Point");
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        switch (playerMovement.speed)
        {
            case PlayerMovement.Speed.Focused:
                spreadDeg = focusedSpreadDeg;
                break;

            case PlayerMovement.Speed.Default:
                spreadDeg = defaultSpreadDeg;
                break;

            case PlayerMovement.Speed.Unfocused:
                spreadDeg = unfocusedSpreadDeg;
                break;
        }

        for (int i = 0; i < bullets.Length && i < 9; i++)
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                selectedBulletIndex = i;

        if (Input.GetKey(KeyCode.J) && Time.time >= cooldown)
        {
            cooldown = Time.time + (1f / fireRate);
            Shoot();
        }
    }

    Vector2 ApplySpread(Vector2 baseDir, float spreadAngleDeg)
    {
        float half = spreadAngleDeg * 0.5f;
        float angle = UnityEngine.Random.Range(-half, half);
        return (Quaternion.Euler(0, 0, angle) * baseDir).normalized;
    }

    public void EquipBullet(int index)
    {
        if (bullets == null || bullets.Length == 0) return;
        selectedBulletIndex = Mathf.Clamp(index, 0, bullets.Length - 1);
    }

    void FireOne(Vector2 dir, Vector2 offset)
    {
        if (bullets == null || bullets.Length == 0) return;
        selectedBulletIndex = Mathf.Clamp(selectedBulletIndex, 0, bullets.Length - 1);

        BulletType type = bullets[selectedBulletIndex];

        GameObject go = Instantiate(type.prefab, firePoint.position - (Vector3)offset, Quaternion.identity);
        go.transform.localScale = Vector3.one * type.scale;
        var pb = go.GetComponent<PlayerBulletSuperClass>();
        if (pb != null)
        {
            pb.Init(dir, type.speed, 10);
        }
    }

    void Shoot()
    {
        Vector2 baseDir = Vector2.up; 

        switch (pattern)
        {
            case ShootPattern.Single:
                {
                    Vector2 dir = ApplySpread(baseDir, spreadDeg);
                    FireOne(dir, new Vector2(0,0));
                    break;
                }

            case ShootPattern.Double:
                {
                    Vector2 d1 = (Quaternion.Euler(0, 0, -spreadDeg / 2) * baseDir).normalized;
                    Vector2 d2 = (Quaternion.Euler(0, 0, spreadDeg / 2) * baseDir).normalized;
                    FireOne(d1, new Vector2(-0.2f, 0));
                    FireOne(d2, new Vector2(0.2f, 0));
                    break;
                }
            case ShootPattern.Tripple:
                {
                    Vector2 d1 = (Quaternion.Euler(0, 0, -spreadDeg / 2) * baseDir).normalized;
                    Vector2 d2 = (Quaternion.Euler(0, 0, spreadDeg / 2) * baseDir).normalized;
                    Vector2 dir = ApplySpread(baseDir, spreadDeg);
                    FireOne(dir, new Vector2(0, 0));
                    FireOne(d1, new Vector2(-0.2f, 0));
                    FireOne(d2, new Vector2(0.2f, 0));
                }
                break;

            case ShootPattern.Tracking:
                break;
        }
    }
}




[System.Serializable]
public struct BulletType
{
    public string name;
    public float scale;
    public float speed;
    public GameObject prefab;
}