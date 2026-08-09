using System;
using UnityEngine;
using Player; 

public class LeafBlowerItem : ItemBase
{
    [Header("Leaf Blower Settings")]
    public float blowbackForce = 40f; // Set this super high to launch yourself!
    
    // Optional: Add a sound or particle effect
    // public GameObject airBlastParticles;
    private void Start()
    {
        totalAmmo = 5;
        currentAmmo = totalAmmo;
    }


    
    
    public override void UseItem(Vector3 startPosition, Vector2 aimDirection)
    {
        if (currentAmmo == 0)
            return;
        
        Debug.Log("WOOSH! Blasted backward!");

        // Optional: Instantiate(airBlastParticles, startPosition, Quaternion.identity);

        PlayerMovement playerMovement = GetComponentInParent<PlayerMovement>();
        if (playerMovement != null)
        {
            // Launch the player away from the mouse!
            playerMovement.AddExternalVelocity(-aimDirection * blowbackForce);
        }
        currentAmmo--;
    }
}