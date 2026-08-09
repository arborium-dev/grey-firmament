using UnityEngine;

public class MeleeItem : ItemBase
{
    public float meleeRange = 1.5f;
    public int damage = 10;

    public override void UseItem(Vector3 startPosition, Vector2 aimDirection)
    {
        Debug.Log("Swung the Melee Weapon!");
        
        // Do a short Raycast or OverlapCircle in the aim direction to hit enemies
        RaycastHit2D hit = Physics2D.Raycast(startPosition, aimDirection, meleeRange);
        
        if (hit.collider != null && hit.collider.CompareTag("Enemy"))
        {
            Debug.Log($"Hit enemy: {hit.collider.name} for {damage} damage!");
            // E.g., hit.collider.GetComponent<EnemyHealth>().TakeDamage(damage);
        }

        if (hit.collider != null && hit.collider.CompareTag("Explosive"))
        {
            
                
                    Debug.Log("SUCCESS: Bullet physically collided with the Explosive barrel!");
            
                    // Get the script
                    ExplosiveBarrelObject barrelScript = hit.collider.GetComponent<ExplosiveBarrelObject>();
            
                    // Check parents just in case
                    if (barrelScript == null) 
                    {
                        barrelScript = hit.collider.GetComponentInParent<ExplosiveBarrelObject>();
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