using UnityEngine;

public class WalkerEnemy : BaseEnemy
{
    public float moveSpeed = 2f;
    private int direction = -1;

    void Update()
    {
        // Stop moving if playing the death animation
        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Walker_Death")) return;
        }

        rb.linearVelocity = new Vector2(direction * moveSpeed, rb.linearVelocity.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrier"))
        {
            direction *= -1;
            sr.flipX = !sr.flipX;
        }

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

            // If Mario stomps from top
            if (contact.normal.y < -0.5f)
            {
                Die();

                collision.gameObject
                    .GetComponent<Rigidbody2D>()
                    .AddForce(Vector2.up * 5f, ForceMode2D.Impulse);
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