using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic; // Needed for Lists

// This lets us create a custom list in the Unity Inspector
[System.Serializable]
public struct ItemLibraryEntry
{
    public string itemName;      // e.g., "Grenade"
    public ItemBase itemPrefab;  // The prefab GameObject holding the item script
}

public class PlayerItemExecuter : MonoBehaviour
{
    // Reference to the PlayerItemManager component
    private PlayerItemManager itemManager;
    private InputAction useItemAction;
    private Camera mainCamera;

    [Header("Item Library (Put ALL items here)")]
    public List<ItemLibraryEntry> allAvailableItems;

    [Header("Hardcoded Melee Slot")]
    public ItemBase meleeWeapon; 

    // We no longer expose these to the inspector; the script fills them automatically
    private ItemBase slotOneItem;
    private ItemBase slotTwoItem;
    private ItemBase slotThreeItem;

    void Start()
    {
        itemManager = GetComponent<PlayerItemManager>();
        mainCamera = Camera.main;

        // Equip the items based on the data passed from the menu!
        EquipLoadout();
    }

    void OnEnable()
    {
        useItemAction = InputSystem.actions.FindAction("UseItem");
    }

    void Update()
    {
        if (useItemAction.WasReleasedThisFrame())
        {
            ExecuteCurrentlySelectedItem();
        }
    }

    private void EquipLoadout()
    {
        // Find and spawn the correct item for each slot
        slotOneItem = SpawnItem(LoadoutManager.slotOneName);
        slotTwoItem = SpawnItem(LoadoutManager.slotTwoName);
        slotThreeItem = SpawnItem(LoadoutManager.slotThreeName);
    }

    private ItemBase SpawnItem(string targetItemName)
    {
        // Search through the library for the matching name
        foreach (ItemLibraryEntry entry in allAvailableItems)
        {
            if (entry.itemName == targetItemName)
            {
                ItemBase spawnedItem = Instantiate(entry.itemPrefab, transform);
                return spawnedItem;
            }
        }

        Debug.LogWarning($"Could not find an item named {targetItemName} in the Library!");
        return null;
    }

    private void ExecuteCurrentlySelectedItem()
    {
        Vector3 startPosition = transform.position;
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, mainCamera.nearClipPlane));
        Vector2 aimDirection = (mouseWorldPosition - startPosition).normalized;

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
                if (meleeWeapon != null) meleeWeapon.UseItem(startPosition, aimDirection);
                break;
        }
    }
}