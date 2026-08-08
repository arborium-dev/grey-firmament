using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(LineRenderer))]
public class PlayerItemUse : MonoBehaviour
{
    private InputAction useItemAction;
    private PlayerItemManager itemManager;
    private string currentlyUsingItem;
    
    private LineRenderer aimLine;
    private Camera mainCamera;
    
    // --- New: Maximum distance the light will travel if it doesn't hit a wall ---
    public float maxLaserDistance = 50f; 

    void Start()
    {
        itemManager = GetComponent<PlayerItemManager>();
        if (itemManager == null)
        {
            itemManager = FindObjectOfType<PlayerItemManager>();
        }

        mainCamera = Camera.main;
        
        aimLine = GetComponent<LineRenderer>();
        aimLine.positionCount = 2; 
        aimLine.enabled = false;   
        
        Color translucentGreen = new Color(0f, 1f, 0f, 0.5f);
        aimLine.startColor = translucentGreen;
        aimLine.endColor = translucentGreen;
        aimLine.startWidth = 0.1f;
        aimLine.endWidth = 0.1f;
    }

    void OnEnable()
    {
        useItemAction = InputSystem.actions.FindAction("UseItem");
    }

    void Update()
    {
        if (itemManager != null)
        {
            currentlyUsingItem = itemManager.CurrentlySelectedItem;
        }
        
        if (useItemAction.WasPressedThisFrame())
        {
            Time.timeScale = 0.3f;
            aimLine.enabled = true; 
            Debug.Log($"Current Item: {currentlyUsingItem}");
        }
        
        if (useItemAction.IsPressed())
        {
            UpdateAimLight(); 
        }

        if (useItemAction.WasReleasedThisFrame())
        {
            Time.timeScale = 1f; 
            aimLine.enabled = false; 
        }
    }

    private void UpdateAimLight()
    {
        // 1. Set Start Point to the Player
        Vector3 startPosition = transform.position;
        aimLine.SetPosition(0, startPosition);

        // 2. Get Mouse World Position
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x, 
            mouseScreenPosition.y, 
            mainCamera.nearClipPlane));
        mouseWorldPosition.z = startPosition.z;

        // 3. Calculate Direction towards the mouse
        Vector2 direction = (mouseWorldPosition - startPosition).normalized;

        // 4. Set a default end position (in case we never hit a wall)
        Vector3 endPosition = startPosition + (Vector3)(direction * maxLaserDistance);

        // 5. Shoot a Raycast in that direction
        // We use RaycastAll so the laser can pass through the player, enemies, or triggers
        // until it specifically finds something tagged "Solid".
        RaycastHit2D[] hits = Physics2D.RaycastAll(startPosition, direction, maxLaserDistance);

        foreach (RaycastHit2D hit in hits)
        {
            // Ignore the player's own collider just in case the ray hits it first
            if (hit.collider.gameObject == this.gameObject) continue;

            // Check if the object we hit has the "Solid" tag
            if (hit.collider.CompareTag("Solid"))
            {
                // Set the end of the line exactly where it hit the wall
                endPosition = hit.point;
                break; // Stop checking further, we hit the wall!
            }
        }

        // 6. Set End Point of the Line Renderer
        aimLine.SetPosition(1, endPosition);
    }
}