using UnityEngine;
using Player;

public class RocketLauncherItem : ItemBase
{
    public GameObject rocketPrefab;
    public float rocketSpeed = 15f;
    private int totalAmmo = 1;
    public int currentAmmo;

    [Header("Recoil Settings")]
    public float recoilForce = 15f; // How hard the gun pushes you back
    
    void Start()
    {
        currentAmmo = totalAmmo;
    }
    public override void UseItem(Vector3 startPosition, Vector2 aimDirection)
    {
        if (currentAmmo <= 0)
        {
            Debug.Log("Out of Ammo");
            return;
        }
        Debug.Log("Fired the Rocket Launcher!");
        
        GameObject rocket = Instantiate(rocketPrefab, startPosition, Quaternion.identity);
        
        // Point the rocket in the direction it's flying
        rocket.transform.up = aimDirection; 
        
        Rigidbody2D rb = rocket.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDirection * rocketSpeed;
        }
        // recoil
        PlayerMovement playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement != null)
        {
            // Push the player in the exact OPPOSITE direction of the aim
            playerMovement.AddExternalVelocity(-aimDirection * recoilForce);
        }
        
        currentAmmo--;
    }
}