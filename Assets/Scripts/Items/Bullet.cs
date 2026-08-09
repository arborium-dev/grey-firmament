using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float lifeTime = 3f; // How long before the bullet deletes itself

    void Start()
    {
        // Fail-safe: Destroy this bullet after 'lifeTime' seconds
        Destroy(gameObject, lifeTime);
    }

    // Because "Is Trigger" is OFF, we use OnCollisionEnter2D
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Notice we have to use "collision.collider" now instead of "hitInfo"

        // 1. Check if we hit an enemy
        if (collision.collider.CompareTag("Enemy"))
        {
            Debug.Log($"Bullet hit an enemy for {damage} damage!");
            
            // EnemyHealth enemy = collision.collider.GetComponent<EnemyHealth>();
            // if (enemy != null) enemy.TakeDamage(damage);

            Destroy(gameObject); 
        }
        // 2. Check if we hit a wall
        else if (collision.collider.CompareTag("Solid"))
        {
            Debug.Log("Bullet hit a wall!");
            Destroy(gameObject);
        }
        // 3. Check if we hit an Explosive barrel
        else if (collision.collider.CompareTag("Explosive"))
        {
            Debug.Log("SUCCESS: Bullet physically collided with the Explosive barrel!");
            
            // Get the script
            ExplosiveBarrelObject barrelScript = collision.collider.GetComponent<ExplosiveBarrelObject>();
            
            // Check parents just in case
            if (barrelScript == null) 
            {
                barrelScript = collision.collider.GetComponentInParent<ExplosiveBarrelObject>();
            }
            
            // Trigger the explosion
            if (barrelScript != null)
            {
                barrelScript.Explode();
            }
            
            // Destroy the bullet
            Destroy(gameObject);
        }
    }
}