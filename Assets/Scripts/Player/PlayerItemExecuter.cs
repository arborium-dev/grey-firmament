using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro; // Needed for the Ammo Text!

[System.Serializable]
public struct ItemLibraryEntry
{
    public string itemName;      
    public ItemBase itemPrefab;  
}

public class PlayerItemExecuter : MonoBehaviour
{
    private PlayerItemManager itemManager;
    private InputAction useItemAction;
    private Camera mainCamera;

    [Header("Item Library")]
    public List<ItemLibraryEntry> allAvailableItems;

    [Header("Hardcoded Melee Slot")]
    public ItemBase meleeWeapon; 

    [Header("Ammo UI Text (Drag Canvas Text Here)")]
    public TextMeshProUGUI slotOneAmmoText;
    public TextMeshProUGUI slotTwoAmmoText;
    public TextMeshProUGUI slotThreeAmmoText;
    public TextMeshProUGUI meleeAmmoText;

    private ItemBase slotOneItem;
    private ItemBase slotTwoItem;
    private ItemBase slotThreeItem;

    void Start()
    {
        itemManager = GetComponent<PlayerItemManager>();
        mainCamera = Camera.main;
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

        // Constantly update the ammo text every frame
        UpdateAmmoUI();
    }

    private void UpdateAmmoUI()
    {
        // 1. Hide all text first so unselected slots are blank
        if (slotOneAmmoText != null) slotOneAmmoText.text = "";
        if (slotTwoAmmoText != null) slotTwoAmmoText.text = "";
        if (slotThreeAmmoText != null) slotThreeAmmoText.text = "";
        if (meleeAmmoText != null) meleeAmmoText.text = "";

        // 2. Only show the text for the Currently Selected item!
        switch (itemManager.CurrentlySelectedItem)
        {
            case "SlotOne":
                UpdateSingleSlotAmmo(slotOneItem, slotOneAmmoText);
                break;
            case "SlotTwo":
                UpdateSingleSlotAmmo(slotTwoItem, slotTwoAmmoText);
                break;
            case "SlotThree":
                UpdateSingleSlotAmmo(slotThreeItem, slotThreeAmmoText);
                break;
            case "Melee":
                UpdateSingleSlotAmmo(meleeWeapon, meleeAmmoText);
                break;
        }
    }

    private void UpdateSingleSlotAmmo(ItemBase item, TextMeshProUGUI ammoText)
    {
        if (item != null && ammoText != null)
        {
            if (item.usesAmmo)
            {
                // Shows like "3 / 3"
                ammoText.text = $"{item.currentAmmo}";
            }
            else
            {
                // If it doesn't use ammo (like a leaf blower), show an infinity symbol!
                ammoText.text = "∞"; 
            }
        }
    }

    private void EquipLoadout()
    {
        slotOneItem = SpawnItem(LoadoutManager.slotOneName);
        slotTwoItem = SpawnItem(LoadoutManager.slotTwoName);
        slotThreeItem = SpawnItem(LoadoutManager.slotThreeName);

        if (slotOneItem != null && slotOneItem.itemIcon != null)
            itemManager.slotOne.sprite = slotOneItem.itemIcon;

        if (slotTwoItem != null && slotTwoItem.itemIcon != null)
            itemManager.slotTwo.sprite = slotTwoItem.itemIcon;

        if (slotThreeItem != null && slotThreeItem.itemIcon != null)
            itemManager.slotThree.sprite = slotThreeItem.itemIcon;

        if (meleeWeapon != null && meleeWeapon.itemIcon != null)
            itemManager.slotMelee.sprite = meleeWeapon.itemIcon;
    }

    private ItemBase SpawnItem(string targetItemName)
    {
        foreach (ItemLibraryEntry entry in allAvailableItems)
        {
            if (entry.itemName == targetItemName)
            {
                ItemBase spawnedItem = Instantiate(entry.itemPrefab, transform);
                return spawnedItem;
            }
        }
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