using UnityEngine;

public class StandstillEnemy : MonoBehaviour
{
    [Header("Targeting Settings")]
    public string playerTag = "Player";
    public float sightRange = 10f;
    public LayerMask obstacleLayer; 

    [Header("Shooting Settings")]
    public GameObject bulletPrefab;
    public Transform firePoint; 
    public float fireRate = 1.5f;
    public float bulletSpeed = 10f;
    
    [Header("Audio Settings")]
    public AudioClip seenSong;      
    public AudioClip unseenSong;    
    
    // --- STATIC VARIABLES (Shared across ALL enemies) ---
    private static AudioSource globalAudio;
    private static int enemiesSeeingPlayerCount = 0;

    // --- LOCAL VARIABLES (Specific to this one enemy) ---
    private bool amISeeingPlayer = false;
    private Transform player;
    private float nextFireTime;

    void Start()
    {
        // 1. If the global audio manager doesn't exist yet, the very first enemy creates it!
        if (globalAudio == null)
        {
            GameObject audioObj = new GameObject("GlobalMusicManager");
            globalAudio = audioObj.AddComponent<AudioSource>();
            globalAudio.loop = true;
            globalAudio.spatialBlend = 0f; // 0 means 2D sound (plays everywhere at the same volume)
            
            enemiesSeeingPlayerCount = 0; // Reset counter at the start of the game
        }

        // 2. Find the player
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
    }

    void Update()
    {
        // If player is dead/missing, this enemy can't see them
        if (player == null) 
        {
            UpdateAlertState(false);
            ManageMusic();
            return; 
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        bool canSeePlayerNow = false;

        if (distanceToPlayer <= sightRange)
        {
            if (CanSeePlayer())
            {
                canSeePlayerNow = true;
            }
        }

        // Update whether THIS specific enemy can see the player
        UpdateAlertState(canSeePlayerNow);
        
        // Decide what music should play globally
        ManageMusic();

        // Shooting logic
        if (canSeePlayerNow && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    // Safely adds or removes THIS enemy from the global count of enemies seeing the player
    void UpdateAlertState(bool canSee)
    {
        // If the enemy JUST spotted the player
        if (canSee && !amISeeingPlayer)
        {
            amISeeingPlayer = true;
            enemiesSeeingPlayerCount++;
        }
        // If the enemy JUST lost sight of the player
        else if (!canSee && amISeeingPlayer)
        {
            amISeeingPlayer = false;
            enemiesSeeingPlayerCount--;
        }
    }

    void ManageMusic()
    {
        // If AT LEAST ONE enemy in the whole game can see the player, play the seen song
        if (enemiesSeeingPlayerCount > 0)
        {
            PlayGlobalSong(seenSong);
        }
        else
        {
            PlayGlobalSong(unseenSong);
        }
    }

    void PlayGlobalSong(AudioClip clipToPlay)
    {
        if (clipToPlay == null || globalAudio == null) return;

        // Only swap the song if a different one is requested
        if (globalAudio.clip != clipToPlay)
        {
            globalAudio.clip = clipToPlay;
            globalAudio.Play();
        }
        else if (!globalAudio.isPlaying)
        {
            globalAudio.Play();
        }
    }

    bool CanSeePlayer()
    {
        Vector2 directionToPlayer = (player.position - transform.position).normalized;
        float distanceToPlayer = Vector2.Distance(transform.position, player.position);

        RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, distanceToPlayer, obstacleLayer);
        return hit.collider == null;
    }

    void Shoot()
    {
        if (bulletPrefab != null && firePoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            
            if (rb != null)
            {
                Vector2 direction = (player.position - firePoint.position).normalized;
                rb.linearVelocity = direction * bulletSpeed;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Floor") || collision.collider.CompareTag("Solid"))
        {
            return; 
        }

        Destroy(gameObject);
    }

    // IMPORTANT: If an enemy dies WHILE looking at the player, it needs to subtract 
    // itself from the counter, otherwise the combat music will play forever!
    void OnDestroy()
    {
        if (amISeeingPlayer)
        {
            enemiesSeeingPlayerCount--;
            amISeeingPlayer = false;
        }
    }
}