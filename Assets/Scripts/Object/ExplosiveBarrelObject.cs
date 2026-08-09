using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps; // Needed to interact with Tilemaps!

public class ExplosiveBarrelObject : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 4f;
    public int explosionDamage = 50;
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Explosive"))
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

    
    
    public void Explode()
    {
        Debug.Log("BOOM!");

        // Optional: Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        // 1. Find everything inside our explosion radius
        Collider2D[] objectsInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInBlast)
        {
            // 2. Damage Enemies in the blast
            if (obj.CompareTag("Enemy"))
            {
                Debug.Log($"Caught {obj.name} in explosion for {explosionDamage} damage!");
                // E.g., obj.GetComponent<EnemyHealth>().TakeDamage(explosionDamage);
            }

            // 3. Destroy Tiles if the object is a Solid Tilemap
            if (obj.CompareTag("Solid"))
            {
                // Try to get the Tilemap component (sometimes it's on a parent object)
                Tilemap tilemap = obj.GetComponent<Tilemap>();
                if (tilemap == null) tilemap = obj.GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    DestroyTilesInRadius(tilemap, transform.position, explosionRadius);
                }
            }

            if (obj.CompareTag("Floor"))
            {
                // Try to get the Tilemap component (sometimes it's on a parent object)
                Tilemap tilemap = obj.GetComponent<Tilemap>();
                if (tilemap == null) tilemap = obj.GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    DestroyTilesInRadius(tilemap, transform.position, (explosionRadius - 1.5f));
                }
            }
        }
    }
    
    void DestroyTilesInRadius(Tilemap tilemap, Vector3 center, float radius)
    {
        // Convert the exact explosion point to a grid coordinate
        Vector3Int centerCell = tilemap.WorldToCell(center);

        // Calculate a bounding box of grid cells based on the radius
        int cellRadius = Mathf.CeilToInt(radius);

        // Loop through a square of cells around the explosion
        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int y = -cellRadius; y <= cellRadius; y++)
            {
                Vector3Int currentCell = centerCell + new Vector3Int(x, y, 0);
                
                // Find the exact world position of this specific tile
                Vector3 cellWorldPos = tilemap.GetCellCenterWorld(currentCell);
                
                // If the distance between the center of the explosion and the tile is less than the radius, BOOM!
                if (Vector2.Distance(center, cellWorldPos) <= radius)
                {
                    // Setting a tile to 'null' deletes it. 
                    // Unity's TilemapCollider2D will automatically update its physics shape!
                    tilemap.SetTile(currentCell, null); 
                }
            }
        }
    }
}
