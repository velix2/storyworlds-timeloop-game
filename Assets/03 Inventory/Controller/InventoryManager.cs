using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Controller Class used to have a handle on what items are in the inventory.
/// </summary>
public class InventoryManager : MonoBehaviour
{
    private InventoryDisplay display;
    [SerializeField] private List<ItemData> items = new List<ItemData>();
    [SerializeField] private List<RecipeData> recipes = new List<RecipeData>();

    public UnityEvent<ItemData> ItemSelected
    {
        get
        {
            display = FindFirstObjectByType<InventoryDisplay>(FindObjectsInactive.Include);
            return display.ItemBoxPrimaryInteract;
        }
    }
    public UnityEvent<ItemData> ItemObserved
    {
        get
        {
            display = FindFirstObjectByType<InventoryDisplay>(FindObjectsInactive.Include);
            return display.ItemBoxSecondaryInteract;
        }
    }
    public bool IsReady => !display.Animating;

    private static InventoryManager instance;
    public static InventoryManager Instance => instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
        
        
    }

    private void Start()
    {

        OnSceneSwitched();
        SceneSwitcher.Instance.SceneSwitched.AddListener(OnSceneSwitched);
    }

    /// <summary>
    /// This is the method you should use if you want to add an Item to Inventory.<br/>
    /// Will also handle displaying the item.
    /// </summary>
    /// <param name="item">Item to be added</param>
    /// <returns></returns>
    public bool AddItem(ItemData item, bool playSound = false)
    {
        if (items.Count == items.Capacity)
        {
            DialogueManager.Instance.EnterDialogueModeSimple("Ich habe keinen Platz mehr.");
            return false;
        }
        
        if (item.MultiplePossible) item = item.MakeCopy();
        items.Add(item);
        display?.AddItemToDisplay(item, playSound);
        return true;
    }

    /// <summary>
    /// This is the method you should use to remove an Item from Inventory.<br/>
    /// Will also handle the removal in display.
    /// </summary>
    /// <param name="item">Item to be removed</param>
    public void RemoveItem(ItemData item)
    {
        items.Remove(item);
        display?.RemoveItemToDisplay(item);
    }

    /// <summary>
    /// Checks if inventory already holds this item.
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public bool HasItem(ItemData item)
    {
        return items.Contains(item);
    }

    /// <summary>
    /// Shows inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void Open()
    {
        display.ShowDisplay();
    }

    /// <summary>
    /// Hide inventory display. Use method from PlayerController to stop player movement.
    /// </summary>
    public void Close()
    {
        display.HideDisplay();
    }

    private void OnAttemptItemCombination(ItemData a, ItemData b)
    {
        Debug.Log("Attempting to combine " + a + " with " + b + "...");
        RecipeData recipe = GetCompatibleRecipe(a, b);
        if (recipe != null)
        {
            Debug.Log("Success! New Item: " + recipe.Product);
            if (recipe.GetsDestroyed1)
            {
                if (recipe.Ingredient1.Equals(a)) RemoveItem(a);
                else RemoveItem(b);
            }

            if (recipe.GetsDestroyed2)
            {
                if (recipe.Ingredient2.Equals(a)) RemoveItem(a);
                else RemoveItem(b);
            }
            AddItem(recipe.Product);
        }
    }

    /// <summary>
    /// Method searches in internal Recipe list for compatible Recipe.
    /// </summary>
    /// <returns>Returns the first Recipe, which uses both given Items.
    /// Will return null if there is none.</returns>
    public RecipeData GetCompatibleRecipe(ItemData a, ItemData b)
    {
        foreach (RecipeData recipe in recipes)
        {
            if (recipe.IngredientsCompatible(a, b)) return recipe;
            
        }
        return null;
    }

    private void OnSceneSwitched()
    {
        display = FindFirstObjectByType<InventoryDisplay>();
        if (display == null)
        {
            Debug.LogError("InventoryDisplay for Inventory Manager was not found.");
        }
        
        items.Capacity = display.Capacity;
        foreach (var itemData in items)
        {
            display.AddItemToDisplay(itemData);
        }
    }
    
}
