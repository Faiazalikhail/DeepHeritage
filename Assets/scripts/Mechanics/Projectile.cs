using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField, Range(0.5f, 10f)] private float lifetime = 10f;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void SetVelocity(Vector2 Velocity)
    {
        GetComponent<Rigidbody2D>().linearVelocity = Velocity;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. Existing Wall Logic
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Destroy(gameObject);
        }

        
        // Check if the object we hit has the "BaseEnemy" script
        BaseEnemy enemy = collision.gameObject.GetComponent<BaseEnemy>();

        if (enemy != null)
        {
            enemy.TakeDamage(); // Deal 1 damage
            Destroy(gameObject); // Destroy the bullet so it doesn't go through
        }
    }
}