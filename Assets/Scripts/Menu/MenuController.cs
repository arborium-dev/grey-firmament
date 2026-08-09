using UnityEngine;
using UnityEngine.SceneManagement; 
using TMPro; 
using UnityEngine.UI; 

public class MenuController : MonoBehaviour
{
    [Header("References")]
    public OptionLoader optionLoader;
    public TextMeshProUGUI instructionText; // Tells player what to do (e.g., "Select Tool 1")
    public string gameSceneNameOne = "GameScene"; // Make sure this matches your exact scene name!
    public string gameSceneNameTwo = "GameSceneTwo";
    public string gameSceneNameThree = "GameSceneThree";
    public string finalSceneName = "FinalScene";
    
    [Header("Transition")]
    public float growDuration = 0.75f;

    [Header("The 5 Buttons")]
    public Button[] optionButtons; //  use this to disable buttons after they are clicked

    private int currentSlotToFill = 1;

    public Image imageSlotOne;
    public Image imageSlotTwo;
    public Image imageSlotThree;
    public Image imageToGrow;

    private bool isTransitioning;

    void Start()
    {
        if (instructionText != null)
        {
            instructionText.text = "Select Tool 1";
        }

        if (GlobalVars.PlayerLevel == 4)
        {
            SceneManager.LoadScene(finalSceneName);
        }
    }

    // This function will be called by the Unity Buttons. 
    // pass in an integer (0 to 4) so the code knows WHICH button was clicked.
    public void SelectOption(int buttonIndex)
    {
        if (optionLoader == null || optionLoader.rolledItemNames == null)
        {
            Debug.LogError("MenuController is missing OptionLoader or rolled items are not initialized.");
            return;
        }

        if (isTransitioning)
        {
            return;
        }

        if (buttonIndex < 0 || buttonIndex >= optionLoader.rolledItemNames.Length)
        {
            Debug.LogError($"SelectOption received invalid button index {buttonIndex}. Expected 0 to {optionLoader.rolledItemNames.Length - 1}.");
            return;
        }

        // 1. Get the name of the item on the clicked button from the OptionLoader
        string chosenItem = optionLoader.rolledItemNames[buttonIndex];
        
        Debug.Log($"Player selected: {chosenItem}");

        Button clickedButton = null;
        if (optionButtons != null && buttonIndex < optionButtons.Length)
        {
            clickedButton = optionButtons[buttonIndex];
        }

        Sprite chosenSprite = null;
        if (optionLoader.rolledItemSprites != null && buttonIndex < optionLoader.rolledItemSprites.Length)
        {
            chosenSprite = optionLoader.rolledItemSprites[buttonIndex];
        }

        // 3. Assign the item to the correct slot
        if (currentSlotToFill == 1)
        {
            LoadoutManager.slotOneName = chosenItem;
            if (imageSlotOne != null && chosenSprite != null)
            {
                imageSlotOne.sprite = chosenSprite;
            }

            currentSlotToFill++;
            if (instructionText != null) instructionText.text = "Select Tool 2";
        }
        else if (currentSlotToFill == 2)
        {
            LoadoutManager.slotTwoName = chosenItem;
            if (imageSlotTwo != null && chosenSprite != null)
            {
                imageSlotTwo.sprite = chosenSprite;
            }
            currentSlotToFill++;
            if (instructionText != null) instructionText.text = "Select Tool 3";
        }
        else if (currentSlotToFill == 3)
        {
            LoadoutManager.slotThreeName = chosenItem;
            if (imageSlotThree != null && chosenSprite != null)
            {
                imageSlotThree.sprite = chosenSprite;
            }

            if (instructionText != null) instructionText.text = "Starting Game...";

            isTransitioning = true;
            SetAllOptionButtonsInteractable(false);

            if (imageToGrow != null)
            {
                imageToGrow.gameObject.SetActive(true);
                imageToGrow.transform.SetAsLastSibling();
                StartCoroutine(GrowAndLoadGame());
            }
            else
            {
                // all 3 slots are filled, load the actual game
                LoadTheActualGame();
            }

            return;
        }
        
        // 2. Disable the button so the player can't pick the exact same item twice
        if (clickedButton != null)
        {
            clickedButton.interactable = false; 
        }
    }

    private void LoadTheActualGame()
    {
        // all 3 slots are filled, load the actual game
        if (GlobalVars.PlayerLevel == 1)
        {
            SceneManager.LoadScene(gameSceneNameOne);
        }
        else if (GlobalVars.PlayerLevel == 2)
        {
            SceneManager.LoadScene(gameSceneNameTwo);
        }
        else if (GlobalVars.PlayerLevel == 3)
        {
            SceneManager.LoadScene(gameSceneNameThree);
        }
    }

    private void SetAllOptionButtonsInteractable(bool interactable)
    {
        if (optionButtons == null)
        {
            return;
        }

        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (optionButtons[i] != null)
            {
                optionButtons[i].interactable = interactable;
            }
        }
    }

    private System.Collections.IEnumerator GrowAndLoadGame()
    {
        if (imageToGrow == null)
        {
            LoadTheActualGame();
            yield break;
        }

        RectTransform growRect = imageToGrow.rectTransform;
        RectTransform canvasRect = imageToGrow.GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();

        if (growRect == null || canvasRect == null)
        {
            LoadTheActualGame();
            yield break;
        }

        Vector2 startSize = growRect.sizeDelta;
        Vector2 targetSize = canvasRect.rect.size;

        imageToGrow.raycastTarget = false;
        imageToGrow.preserveAspect = false;

        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            growRect.sizeDelta = Vector2.Lerp(startSize, targetSize, t);
            yield return null;
        }

        growRect.sizeDelta = targetSize;
        LoadTheActualGame();
    }
}