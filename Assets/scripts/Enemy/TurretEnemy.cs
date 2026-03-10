using UnityEngine;

public class TurretEnemy : BaseEnemy
{
    [Header("Shooting Settings")]
    public float detectionRange = 10f;
    public float fireRate = 2f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    private float nextFireTime = 0f;
    private Transform player;

    public override void Start()
    {
        base.Start();
        GameObject mario = GameObject.FindWithTag("Player");
        if (mario != null) player = mario.transform;
    }

    void Update()
    {
        if (player == null) return;

        // Face the Player
        if (player.position.x > transform.position.x)
        {
            sr.flipX = false;
        }
        else
        {
            sr.flipX = true;
        }

        // Check Range & Shoot
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectionRange)
        {
            if (Time.time >= nextFireTime)
            {
                Shoot();
                nextFireTime = Time.time + fireRate;
            }
        }
    }

    void Shoot()
    {
        if (projectilePrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            EnemyProjectile bulletScript = bullet.GetComponent<EnemyProjectile>();

            
            if (sr.flipX == false)
            {
                bulletScript.speed = -Mathf.Abs(bulletScript.speed); 
            }
            else
            {
                bulletScript.speed = Mathf.Abs(bulletScript.speed);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Die();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            if (contact.normal.y < -0.5f)
            {
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
            }
            else
            {
                PlayerController mario = collision.gameObject.GetComponent<PlayerController>();
                if (mario != null)
                {
                    Vector2 hitDirection = (collision.transform.position - transform.position).normalized;
                    mario.TakeDamage(hitDirection);
                }
            }
        }
    }
}