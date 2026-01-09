using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public enum ShootPattern { Single, Double, Tracking}
    public ShootPattern pattern;
    Transform firePoint;
    float cooldown;
    float fireRate;
    public BulletTypes[] bullets;
    float bulletSpread;
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
                bulletSpread = 0;
                break;

            case PlayerMovement.Speed.Unfocused:
                bulletSpread = 1;
                break;

            default:
                bulletSpread = 0.5f;
                break;
        }
        if (Input.GetKey(KeyCode.J) && Time.time >= cooldown)
        {
            cooldown = Time.time + 1f / fireRate;
            Shoot();
        }
    }

    public void SetPattern(ShootPattern newPattern) => pattern = newPattern;

    void Shoot()
    {
        switch (pattern)
        {
            case ShootPattern.Single:
                break;
            case ShootPattern.Tracking:
                break;
            case ShootPattern.Double:
                break;
        }
    }
}

[System.Serializable]
public struct BulletTypes
{
    public string name;
    public float scale;
    public GameObject bullet;
}