using UnityEngine;

[RequireComponent(typeof(SpriteRenderer), typeof(Animator), typeof(Rigidbody2D))]
public class BaseEnemy : MonoBehaviour
{
    protected SpriteRenderer sr;
    protected Animator anim;
    protected Rigidbody2D rb;

    [Header("Health Settings")]
    public int maxHealth = 1;

    // --- LAB 9 AUDIO ---
    [Header("Audio Settings")]
    public AudioClip enemyDeathSound;
    // -------------------

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

    public virtual void Die()
    {
        this.enabled = false;

        
        if (enemyDeathSound != null)
        {
            AudioSource.PlayClipAtPoint(enemyDeathSound, Camera.main.transform.position);
        }

        foreach (Collider2D col in GetComponents<Collider2D>())
        {
            col.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 3f;
            rb.AddForce(new Vector2(2f, 10f), ForceMode2D.Impulse);
        }

        if (sr != null) sr.flipY = true;

        Destroy(gameObject, 3f);
    }
}