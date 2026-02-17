using UnityEngine;

public class WalkerEnemy : BaseEnemy
{
    public float moveSpeed = 2f;
    private int direction = -1;

    void Update()
    {
        AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Walker_Death")) return;

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            direction *= -1;
            sr.flipX = !sr.flipX;
        }

        // Also die to bullets like the Turret
        if (collision.CompareTag("Projectile"))
        {
            Destroy(collision.gameObject);
            Die();
        }
    }

    // STOMP LOGIC 
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Check if Mario is falling downwards (Stomping)
            // AND is physically above the enemy
            ContactPoint2D contact = collision.GetContact(0);

            // If the hit comes from the TOP (Normal points up)
            if (contact.normal.y < -0.5f)
            {
                Die();
                // Bounce Mario up a little
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
            }
            else
            {
                // Mario hit the side -> Mario takes damage
                PlayerController mario = collision.gameObject.GetComponent<PlayerController>();
                if (mario != null) mario.TakeDamage();
            }
        }
    }
}