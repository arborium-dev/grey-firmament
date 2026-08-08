using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerItemManager : MonoBehaviour
{
    private InputAction selectSlotOneAction;
    private InputAction selectSlotTwoAction;
    private InputAction selectSlotThreeAction;
    private InputAction selectSlotMeleeAction;
    private PlayerInput playerInput;

    public Image slotOne;
    public Image slotTwo;
    public Image slotThree;
    public Image slotMelee;
    
    public string CurrentlySelectedItem;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CurrentlySelectedItem = "Melee";
        UpdateSlotGUI();
    }

    private void OnEnable()
    {

        selectSlotOneAction = InputSystem.actions.FindAction("SelectSlotOne");
        selectSlotTwoAction = InputSystem.actions.FindAction("SelectSlotTwo");
        selectSlotThreeAction = InputSystem.actions.FindAction("SelectSlotThree");
        selectSlotMeleeAction = InputSystem.actions.FindAction("SelectSlotMelee");
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 1f)
        {
            if (selectSlotOneAction.WasPerformedThisFrame())
            {
                SelectItem("SlotOne", slotOne);
            }

            if (selectSlotTwoAction.WasPerformedThisFrame())
            {
                SelectItem("SlotTwo", slotTwo);
            }

            if (selectSlotThreeAction.WasPerformedThisFrame())
            {
                SelectItem("SlotThree", slotThree);
            }

            if (selectSlotMeleeAction.WasPerformedThisFrame())
            {
                SelectItem("Melee", slotMelee);
            }
        }
    }

    void SelectItem(string itemName, Image slotImage)
    {
        CurrentlySelectedItem = itemName;
        UpdateSlotGUI();
    }

    void UpdateSlotGUI()
    {
        // Reset all slots to default color
        slotOne.color = Color.white;
        slotTwo.color = Color.white;
        slotThree.color = Color.white;
        slotMelee.color = Color.white;
        
        // Highlight the currently selected slot
        switch (CurrentlySelectedItem)
        {
            case "SlotOne":
                slotOne.color = Color.green;
                break;
            case "SlotTwo":
                slotTwo.color = Color.green;
                break;
            case "SlotThree":
                slotThree.color = Color.green;
                break;
            case "Melee":
                slotMelee.color = Color.green;
                break;
        }
    }
}
