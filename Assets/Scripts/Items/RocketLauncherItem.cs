using UnityEngine;

public class RocketLauncherItem : ItemBase
{
    public GameObject rocketPrefab;
    public float rocketSpeed = 15f;
    private int totalAmmo = 1;
    public int currentAmmo;

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
        currentAmmo--;
    }
}