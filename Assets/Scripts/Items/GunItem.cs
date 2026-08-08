using UnityEngine;
using Player;

public class GunItem : ItemBase
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public int totalAmmo = 6;
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
            Debug.Log("Out of ammo!");
            return;
        }
        Debug.Log("Fired the Gun!");
        // Create the bullet
        GameObject bullet = Instantiate(bulletPrefab, startPosition, Quaternion.identity);
        
        bullet.transform.right = aimDirection; 

        // Shoot it in the aim direction
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDirection * bulletSpeed;
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