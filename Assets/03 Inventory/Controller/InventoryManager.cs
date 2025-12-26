using System.Collections.Generic;
using System.Linq;
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

    public UnityEvent<ItemData> ItemSelected => display.ItemBoxPrimaryInteract;
    public UnityEvent<ItemData> ItemObserved => display.ItemBoxSecondaryInteract;

    private void Awake()
    {
        display = FindFirstObjectByType<InventoryDisplay>();
        if (display == null)
        {
            Debug.LogError("InventoryDisplay for Inventory Manager was not found.");
        }
        
        ItemData.AttemptItemCombination.AddListener(OnAttemptItemCombination);
        
    }

    private void Start()
    {
        foreach (var itemData in items)
        {
            display.AddItemToDisplay(itemData);
        }
        
    }

    /// <summary>
    /// This is the method you should use if you want to add an Item to Inventory.<br/>
    /// Will also handle displaying the item.
    /// </summary>
    /// <param name="item">Item to be added</param>
    /// <returns></returns>
    public bool AddItem(ItemData item)
    {
        if (item.MultiplePossible) item = item.MakeCopy();
        items.Add(item);
        display.AddItemToDisplay(item);
        
        //TODO: capacity check
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
        display.RemoveItemToDisplay(item);
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
    
}
