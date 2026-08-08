using UnityEngine;
using Player;

public class GrenadeItem : ItemBase
{
    public GameObject grenadePrefab;
    public float throwSpeed = 12f;
    private int totalAmmo = 3; // You get a set of 3 grenades
    public int currentAmmo;

    [Header("Recoil Settings")]
    public float recoilForce = 5f; // Smaller recoil for throwing
    
    void Start()
    {
        currentAmmo = totalAmmo;
    }

    public override void UseItem(Vector3 startPosition, Vector2 aimDirection)
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of Grenades");
            return;
        }
        
        Debug.Log("Threw a Grenade!");
        
        GameObject grenade = Instantiate(grenadePrefab, startPosition, Quaternion.identity);
        
        Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Throw the grenade in the aim direction
            rb.linearVelocity = aimDirection * throwSpeed;
            
            // give it a random spin so it looks like it's tumbling
            rb.angularVelocity = Random.Range(-300f, 300f);
        }
        
        // Recoil
        PlayerMovement playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.AddExternalVelocity(-aimDirection * recoilForce);
        }
        
        currentAmmo--;
    }
}