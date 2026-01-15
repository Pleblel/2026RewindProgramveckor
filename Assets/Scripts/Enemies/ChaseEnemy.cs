using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ChaseMovement : EnemyEntity
{
    //Darren Scott


    [SerializeField] GameObject playerTarget;
    [SerializeField] bool reachedPosition;
    bool firedBullet = false;
    bool hasBeenVisible;
    [SerializeField] GameObject runawayTarget;
    [SerializeField] float stayTimer = 2f;
    [SerializeField] GameObject bullet;
    [SerializeField] GameObject sprite;
    [SerializeField] float distanceStop;

    // Start is called before the first frame update
    void Start()
    {
        FindPlayer();
        distanceStop = Random.Range(2, 6);
        enemyHP = 40f;
        currentHP = enemyHP;
        movementSpeed = 5f;
        stopDistance = distanceStop;
        target = playerTarget.transform.position;

        if (DifficultyManager.I != null)
            movementSpeed *= DifficultyManager.I.MoveSpeedMult;
    }

    // Update is called once per frame
    void Update()
    {
        Movement(movementSpeed);
        FacePoint(playerTarget.transform.position);

        if (reachedPosition)
        {
            stayTimer -= Time.deltaTime;

            if (!firedBullet)
            {
                FireBullet();
                firedBullet = true;
            }

            if (stayTimer <= 0f)
            {
                RunAway(movementSpeed);
                FacePoint(target);
            }
                
        }
           
    }

    protected override void Movement(float speed)
    {

        Vector2 pos = transform.position;

        if (playerTarget == null)
        {
            return;
        }

        if (Vector2.Distance(pos, target) <= stopDistance)
        {
            reachedPosition = true; 
            return;
        }
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }


    void RunAway(float speed)
    {

        Vector2 pos = transform.position;

        target = runawayTarget.transform.position;

        if (runawayTarget == null)
            return; 

       transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);
    }
    void FindPlayer()
    {
        playerTarget = GameObject.FindGameObjectWithTag("Player"); 
    }

    void FacePoint(Vector2 point)
    {
        Vector2 dir = (point - (Vector2)transform.position);
        if (dir.sqrMagnitude < 0.0001f) return;

        // If your sprite faces UP by default:
       sprite.transform.up = dir.normalized;

        // If your sprite faces RIGHT by default, use this instead:
        // transform.right = dir.normalized;
    }

    void FireBullet()
    {
        Instantiate(bullet, transform.position, Quaternion.identity);
    }

    private void OnBecameVisible()
    {
        hasBeenVisible = true; 
    }

    private void OnBecameInvisible()
    {
        if (hasBeenVisible)
            Destroy(gameObject);
    }
}
