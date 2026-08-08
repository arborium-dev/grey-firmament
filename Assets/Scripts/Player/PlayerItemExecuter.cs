using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerItemExecutor : MonoBehaviour
{
    private PlayerItemManager itemManager;
    private InputAction useItemAction;
    private Camera mainCamera;

    [Header("Dynamic Inventory Slots")]
    public ItemBase slotOneItem;
    public ItemBase slotTwoItem;
    public ItemBase slotThreeItem;

    [Header("Hardcoded Melee Slot")]
    public ItemBase meleeWeapon; // This will always hold your MeleeItem script

    void Start()
    {
        itemManager = GetComponent<PlayerItemManager>();
        mainCamera = Camera.main;
    }

    void OnEnable()
    {
        // Grabbing the exact same input action your PlayerItemUse script uses
        useItemAction = InputSystem.actions.FindAction("UseItem");
    }

    void Update()
    {
        // When the player lets go of the button, trigger the item
        if (useItemAction.WasReleasedThisFrame())
        {
            ExecuteCurrentlySelectedItem();
        }
    }

    private void ExecuteCurrentlySelectedItem()
    {
        // Calculate the direction just like you did in PlayerItemUse
        Vector3 startPosition = transform.position;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        Vector2 aimDirection = (mouseWorldPosition - startPosition).normalized;

        // Check which string is active in your Manager, and fire the matching item
        switch (itemManager.CurrentlySelectedItem)
        {
            case "SlotOne":
                if (slotOneItem != null) slotOneItem.UseItem(startPosition, aimDirection);
                break;

            case "SlotTwo":
                if (slotTwoItem != null) slotTwoItem.UseItem(startPosition, aimDirection);
                break;

            case "SlotThree":
                if (slotThreeItem != null) slotThreeItem.UseItem(startPosition, aimDirection);
                break;

            case "Melee":
                // Melee is permanently hardcoded here
                if (meleeWeapon != null) meleeWeapon.UseItem(startPosition, aimDirection);
                break;
        }
    }
}