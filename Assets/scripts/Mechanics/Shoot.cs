using UnityEngine;

public class Shoot : MonoBehaviour
{
    private SpriteRenderer _sr;
    [SerializeField] private Vector2 initalShotVelocity = new Vector2(3, 3);
    [SerializeField] private Transform spawnPointLeft;
    [SerializeField] private Transform spawnPointRight;
    [SerializeField] private Projectile projectilePrefab;

    void Start()
    {
        _sr = GetComponent<SpriteRenderer>();

        if (initalShotVelocity == Vector2.zero)
        {
            initalShotVelocity = new Vector2(3, 3);
        }
    }

    // --- THIS IS THE NEW PART ---
    //void Update()
    //{
    //    // When you press the Fire button (Left Ctrl or Mouse Click)...
    //    if (Input.GetButtonDown("Fire1"))
    //    {
    //        // ...run the Fire function below!
    //        Fire();
    //    }
    //}
    // -----------------------------

    public void Fire()
    {
        Projectile currentProjectile;
        if (!_sr.flipX)
        {
            currentProjectile = Instantiate(projectilePrefab, spawnPointRight.position, Quaternion.identity);
            currentProjectile.SetVelocity(initalShotVelocity);
        }
        else
        {
            currentProjectile = Instantiate(projectilePrefab, spawnPointLeft.position, Quaternion.identity);
            currentProjectile.SetVelocity(new Vector2(-initalShotVelocity.x, initalShotVelocity.y));
        }
    }
}