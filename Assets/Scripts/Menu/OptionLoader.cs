using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public struct ItemSelectorLibraryEntry
{
    public string itemName;      // e.g., "Grenade"
    public string shownItemDescription;
    public Sprite shownItemSprite; // Changed from Image to Sprite!
}

public class OptionLoader : MonoBehaviour
{
    [Header("The Full Item Pool")]
    public List<ItemSelectorLibraryEntry> fullItemPool;

    [Header("UI Text Slots")]
    public TextMeshProUGUI optionOneText;
    public TextMeshProUGUI optionTwoText;
    public TextMeshProUGUI optionThreeText;
    public TextMeshProUGUI optionFourText;
    public TextMeshProUGUI optionFiveText;
    
    [Header("UI Image Slots")]
    public Image optionOneImage;
    public Image optionTwoImage;
    public Image optionThreeImage;
    public Image optionFourImage;
    public Image optionFiveImage;

    // We will store the names of the 5 rolled items here, so when you click 
    // a button later to select one, the game knows which item was in that slot!
    [HideInInspector] public string[] rolledItemNames = new string[5];

    void Start()
    {
        LoadRandomOptions();
    }

    public void LoadRandomOptions()
    {
        // We need at least some items in the pool to do this
        if (fullItemPool == null || fullItemPool.Count == 0)
        {
            Debug.LogWarning("Item pool is empty! Add items in the inspector.");
            return;
        }

        // Create a temporary copy of the pool so we can shuffle it without ruining the original
        List<ItemSelectorLibraryEntry> shuffledPool = new List<ItemSelectorLibraryEntry>(fullItemPool);

        // shuuffle the list randomly 
        for (int i = 0; i < shuffledPool.Count; i++)
        {
            ItemSelectorLibraryEntry temp = shuffledPool[i];
            int randomIndex = Random.Range(i, shuffledPool.Count);
            shuffledPool[i] = shuffledPool[randomIndex];
            shuffledPool[randomIndex] = temp;
        }

        // UI slots in a quick array so loop 

        TextMeshProUGUI[] textSlots = { optionOneText, optionTwoText, optionThreeText, optionFourText, optionFiveText };
        Image[] imageSlots = { optionOneImage, optionTwoImage, optionThreeImage, optionFourImage, optionFiveImage };

        // Pick the first 5 items from the shuffled list
        int itemsToDisplay = Mathf.Min(5, shuffledPool.Count);

        for (int i = 0; i < itemsToDisplay; i++)
        {
            // Apply the text and sprite to the UI
            textSlots[i].text = shuffledPool[i].shownItemDescription;
            imageSlots[i].sprite = shuffledPool[i].shownItemSprite;
            
            // Make sure the image is fully visible (opacity 100%)
            imageSlots[i].color = Color.white;

            // Save the name of the item that got put into this slot
            rolledItemNames[i] = shuffledPool[i].itemName;
        }

        // fail-safe: If you have less than 5 items, hide the unused UI slots
        for (int i = itemsToDisplay; i < 5; i++)
        {
            textSlots[i].text = "???";
            imageSlots[i].color = Color.clear; // Make the image invisible
            rolledItemNames[i] = "";
        }
    }
}