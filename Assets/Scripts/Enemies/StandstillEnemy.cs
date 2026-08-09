using UnityEngine;

public class StandstillEnemy : MonoBehaviour
{
    [Header("Targeting Settings")]
    public string playerTag = "Player";
    public float sightRange = 10f;
    // Set this layer mask in the inspector to the layers that contain your walls/floors 
    // so the enemy can't shoot through them!
    public LayerMask obstacleLayer; 

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; // Create an empty GameObject child for where the bullet spawns
    public float fireRate = 1.5f;
    public float bulletSpeed = 10f;
    
    private Transform player;
    private float nextFireTime;

    void Start()
    {
        // Find the player automatically at the start of the game
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // If the player is missing or destroyed, do nothing
        if (player == null) return; 

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // 1. Check if the player is close enough
        if (distanceToPlayer <= sightRange)
        {
            // 2. Check if the enemy can physically see the player (no walls in the way)
            if (CanSeePlayer())
            {
                // 3. Check if enough time has passed to shoot again
                if (Time.time >= nextFireTime)
                {
                    Shoot();
                    nextFireTime = Time.time + 1f / fireRate;
                }
            }
        }
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        // Cast a ray towards the player looking ONLY for obstacles (walls, floors)
        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);

        // If the ray didn't hit any walls/solids, we have a clear line of sight
        return hit.collider == null;
    }

    void Shoot()
    {
        Debug.Log("Enemy spotted player! Firing bullet.");

        if (bulletPrefab != null && firePoint != null)
        {
            // Spawn the bullet
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            
            // Push the bullet towards the player
            if (rb != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // We want the enemy to die if hit by anything EXCEPT the environment.
        // Without this check, the enemy would instantly die upon touching the floor!
        if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Solid"))
        {
            return; 
        }

        // If it's a bullet, explosion, player, etc. -> Destroy the enemy


        Debug.Log($"Enemy hit by {collision.gameObject.name}! Removing enemy from scene.");
        Destroy(gameObject);
    }
}