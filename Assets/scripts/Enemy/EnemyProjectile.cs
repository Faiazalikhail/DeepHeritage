using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float speed = 5f;

    void Start()
    {
        Destroy(gameObject, 3f); // Destroy after 3 seconds
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // 1. Hurt Mario
            PlayerController mario = collision.GetComponent<PlayerController>();
            if (mario != null)
            {
                mario.TakeDamage();
            }

            // 2. Destroy the bullet
            Destroy(gameObject);
        }
    }
}