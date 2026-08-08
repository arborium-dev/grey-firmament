using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.UI; 

public class MenuController : MonoBehaviour
{
    [Header("References")]
    public OptionLoader optionLoader;
    public TextMeshProUGUI instructionText; // Tells player what to do (e.g., "Select Tool 1")
    public string gameSceneName = "GameScene"; // Make sure this matches your exact scene name!

    [Header("The 5 Buttons")]
    public Button[] optionButtons; //  use this to disable buttons after they are clicked

    private int currentSlotToFill = 1;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Select Tool 1";
        }
    }

    // This function will be called by the Unity Buttons. 
    // pass in an integer (0 to 4) so the code knows WHICH button was clicked.
    public void SelectOption(int buttonIndex)
    {
        // 1. Get the name of the item on the clicked button from the OptionLoader
        string chosenItem = optionLoader.rolledItemNames[buttonIndex];
        
        Debug.Log($"Player selected: {chosenItem}");

        // 2. Disable the button so the player can't pick the exact same item twice
        if (optionButtons.Length > buttonIndex && optionButtons[buttonIndex] != null)
        {
            optionButtons[buttonIndex].interactable = false; 
        }

        // 3. Assign the item to the correct slot
        if (currentSlotToFill == 1)
        {
            LoadoutManager.slotOneName = chosenItem;
            currentSlotToFill++;
            if (instructionText != null) instructionText.text = "Select Tool 2";
        }
        else if (currentSlotToFill == 2)
        {
            LoadoutManager.slotTwoName = chosenItem;
            currentSlotToFill++;
            if (instructionText != null) instructionText.text = "Select Tool 3";
        }
        else if (currentSlotToFill == 3)
        {
            LoadoutManager.slotThreeName = chosenItem;
            
            if (instructionText != null) instructionText.text = "Starting Game...";
            
            // all 3 slots are filled, load the actual game
            SceneManager.LoadScene(gameSceneName);
        }
    }
}