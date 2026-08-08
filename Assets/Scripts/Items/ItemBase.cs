using UnityEngine;

public abstract class ItemBase : MonoBehaviour
{
    [Header("UI Information")]
    public Sprite itemIcon; 

    [Header("Ammo Settings")]
    public bool usesAmmo = false; // Check this for rockets/grenades. Leave false for LeafBlower/Melee.
    public int currentAmmo;
    public int totalAmmo;

    // Every item needs a Use function
    public abstract void UseItem(Vector3 startPosition, Vector2 aimDirection);
}