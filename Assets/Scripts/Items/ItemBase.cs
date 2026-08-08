using UnityEngine;

// This is an abstract class. You won't attach this directly to anything,
// instead, your specific items will inherit from this.
public abstract class ItemBase : MonoBehaviour
{
    // Every item needs a Use function that takes the starting position and aiming direction
    public abstract void UseItem(Vector3 startPosition, Vector2 aimDirection);
}