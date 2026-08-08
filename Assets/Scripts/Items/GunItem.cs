using UnityEngine;

public class GunItem : ItemBase
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 40f;
    public int totalAmmo = 6;
    public int currentAmmo;

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
        
        // Shoot it in the aim direction
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = aimDirection * bulletSpeed;
        }
        currentAmmo--;
    }
}