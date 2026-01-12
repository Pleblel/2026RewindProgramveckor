using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBullet : MonoBehaviour
{

    [SerializeField] GameObject player;
    Vector2 movedir;
    [SerializeField] float bulletSpeed = 6f;
    private void Awake()
    {
       
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        movedir = ((Vector2)player.transform.position - (Vector2)transform.position).normalized;
    }

    void Update()
    {
        Fire(movedir, bulletSpeed);
    }

    public void Fire(Vector2 dir, float speed)
    {
        transform.position += (Vector3)(dir * speed * Time.deltaTime);

    }
}
