using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Health Settings")]
    public int maxHealth = 1;

    public virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    public virtual void TakeDamage(int amount = 1)
    {
        maxHealth -= amount;
        if (maxHealth <= 0)
        {
            Die();
        }
    }

    // DIE FUNCTION 
    public virtual void Die()
    {
        
        // Disables this script so Walk/Shoot logic stops running immediately
        this.enabled = false;

        
        // This lets them fall through the floor instead of getting stuck
        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        // 3. Physics Launch
        if (rb != null)
        {
            // Reset velocity first
            rb.linearVelocity = Vector2.zero;

            // Important: Turrets are Kinematic, so we MUST switch them to Dynamic to make them fall
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f; // Make them fall fast

            // Add a "Pop" force: Up (10) and slightly Right (2)
            rb.AddForce(new Vector2(2f, 10f), ForceMode2D.Impulse);
        }

        
        // Flips the sprite upside down like in Mario
        if (sr != null) sr.flipY = true;

        // 5. Destroy after 3 seconds ⏳
        Destroy(gameObject, 3f);
    }
}