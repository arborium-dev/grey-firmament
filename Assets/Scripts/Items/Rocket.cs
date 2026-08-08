using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps; // Needed to interact with Tilemaps!

public class Rocket : MonoBehaviour
{
    [Header("Explosion Settings")]
    public float explosionRadius = 2.5f;
    public int explosionDamage = 50;
    public float lifeTime = 5f;
    
    // Optional: Add an explosion particle effect prefab here
    // public GameObject explosionEffectPrefab; 

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D hitInfo)
    {
        // Ignore the player so we don't blow ourselves up instantly
        if (hitInfo.CompareTag("Player")) return;

        // If it hits a Wall OR an Enemy, explode!
        if (hitInfo.CompareTag("Solid") || hitInfo.CompareTag("Enemy"))
        {
            Explode();
            Destroy(gameObject); // Destroy the rocket itself
        }
    }

    void Explode()
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