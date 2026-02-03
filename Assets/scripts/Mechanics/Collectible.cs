using UnityEngine;

public class Collectible : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Log exactly what we just touched
            Debug.Log("COLLECTED: " + gameObject.tag);

            PlayerController mario = other.GetComponent<PlayerController>();

            if (mario != null)
            {
                if (gameObject.CompareTag("RedPU"))
                {
                    mario.ActivateSpeedBoost();
                }
                else if (gameObject.CompareTag("GreenPU"))
                {
                    mario.ActivateJumpBoost();
                }
                else if (gameObject.CompareTag("BluePU"))
                {
                    mario.ActivateFloatBoost();
                }
            }

            Destroy(gameObject);
        }
    }
}