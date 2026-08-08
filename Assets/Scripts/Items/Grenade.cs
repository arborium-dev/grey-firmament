using UnityEngine;
using UnityEngine.Tilemaps;

public class Grenade : MonoBehaviour
{
    [Header("Grenade Settings")]
    public float fuseTime = 2f; // How long before it blows up
    public float explosionRadius = 3f;
    public int explosionDamage = 40;
    
    // public GameObject explosionEffectPrefab; 

    void Start()
    {
        // Start the fuse timer! This calls the Explode() function after 'fuseTime' seconds.
        Invoke(nameof(Explode), fuseTime);
    }

    // REMOVED OnTriggerEnter2D. 
    // We want the grenade to bounce off walls

    void Explode()
    {
        Debug.Log("GRENADE BOOM!");

        // Instantiate(explosionEffectPrefab, transform.position, Quaternion.identity);

        Collider2D[] objectsInBlast = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (Collider2D obj in objectsInBlast)
        {
            if (obj.CompareTag("Enemy"))
            {
                Debug.Log($"Caught {obj.name} in grenade explosion for {explosionDamage} damage!");
                // obj.GetComponent<EnemyHealth>().TakeDamage(explosionDamage);
            }

            // GRENADE LOGIC: Walls take LESS damage (smaller radius)
            if (obj.CompareTag("Solid"))
            {
                Tilemap tilemap = obj.GetComponent<Tilemap>();
                if (tilemap == null) tilemap = obj.GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    // Subtracting 1.5f means it destroys fewer wall tiles
                    DestroyTilesInRadius(tilemap, transform.position, (explosionRadius - 1.5f));
                }
            }

            // GRENADE LOGIC: Floor takes MORE damage (full radius)
            if (obj.CompareTag("Floor"))
            {
                Tilemap tilemap = obj.GetComponent<Tilemap>();
                if (tilemap == null) tilemap = obj.GetComponentInParent<Tilemap>();

                if (tilemap != null)
                {
                    // Full radius for floors
                    DestroyTilesInRadius(tilemap, transform.position, explosionRadius);
                }
            }
        }

        // Destroy the grenade object after it explodes
        Destroy(gameObject);
    }

    void DestroyTilesInRadius(Tilemap tilemap, Vector3 center, float radius)
    {
        Vector3Int centerCell = tilemap.WorldToCell(center);
        int cellRadius = Mathf.CeilToInt(radius);

        for (int x = -cellRadius; x <= cellRadius; x++)
        {
            for (int y = -cellRadius; y <= cellRadius; y++)
            {
                Vector3Int currentCell = centerCell + new Vector3Int(x, y, 0);
                Vector3 cellWorldPos = tilemap.GetCellCenterWorld(currentCell);
                
                if (Vector2.Distance(center, cellWorldPos) <= radius)
                {
                    tilemap.SetTile(currentCell, null); 
                }
            }
        }
    }
}