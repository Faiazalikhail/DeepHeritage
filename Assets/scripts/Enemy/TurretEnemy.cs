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

        // 1. Face the Player (Flip Logic)
        if (player.position.x > transform.position.x)
        {
            sr.flipX = false; // Face Right (assuming sprite faces left by default)
        }
        else
        {
            sr.flipX = true; // Face Left
        }

        // 2. Check Range & Shoot
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
            // 1. Create the bullet
            GameObject bullet = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);

            // 2. Get the bullet's script
            EnemyProjectile bulletScript = bullet.GetComponent<EnemyProjectile>();

            // 3. FORCE DIRECTION
            // If the Turret sprite is flipped (facing RIGHT), bullet goes RIGHT (Positive Speed)
            if (sr.flipX == true)
            {
                bulletScript.speed = Mathf.Abs(bulletScript.speed); // POSITIVE (Right)
            }
            else
            {
                // If Turret is normal (facing LEFT), bullet goes LEFT (Negative Speed)
                bulletScript.speed = -Mathf.Abs(bulletScript.speed); // NEGATIVE (Left)
            }
        }
    }

    // 3. DIE when hit by Mario's Projectile
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Assuming Mario's bullet is tagged "Projectile" or "PlayerProjectile"
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject); // Destroy the bullet
            Die(); // Die instantly
        }
    }
}