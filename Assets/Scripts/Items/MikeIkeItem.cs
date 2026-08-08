using UnityEngine;
using Player;

public class MikeIkeItem : ItemBase
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log("doesnt do anything");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void UseItem(Vector3 startPosition, Vector2 aimDirection)
    {
        Debug.Log("does anything");
    }
}
