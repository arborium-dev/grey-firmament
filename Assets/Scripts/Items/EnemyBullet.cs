using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D collision)
    {
        // If the bullet collides with the player, remove the player from the scene
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player was hit! Removing player.");
            Destroy(collision.gameObject);
        } else if (collision.collider.CompareTag("Explosive"))
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

        // Destroy the bullet when it hits anything (player, walls, floors, etc.)
        Destroy(gameObject);
    }
}