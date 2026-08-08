using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 3f; // How long before the bullet deletes itself

    void Start()
    {
        // Fail-safe: Destroy this bullet after 'lifeTime' seconds so 
        // they don't float through space forever and lag your game.
        Destroy(gameObject, lifeTime);
    }

    // This function runs automatically when the bullet's trigger collider hits another collider
    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // 1. Check if we hit an enemy
        if (hitInfo.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet hit an enemy for {damage} damage!");
            
            // Example of how you would deal damage if your enemy has a health script:
            // EnemyHealth enemy = hitInfo.GetComponent<EnemyHealth>();
            // if (enemy != null) enemy.TakeDamage(damage);

            // Destroy the bullet after hitting the enemy
            Destroy(gameObject); 
        }

        // 2. Check if we hit a wall (Using the same "Solid" tag from your Laser script)
        else if (hitInfo.CompareTag("Solid"))
        {
            Debug.Log("Bullet hit a wall!");
            
            // Destroy the bullet so it doesn't fly through the wall
            Destroy(gameObject);
        }
    }
}