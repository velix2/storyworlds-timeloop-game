using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Resource/RecipeData", fileName = "New RecipeData")][Serializable]
public class RecipeData : ScriptableObject
{
    [SerializeField] private ItemData ingredient1;
    [SerializeField] private bool getsDestroyed1;
    [SerializeField] private ItemData ingredient2;
    [SerializeField] private bool getsDestroyed2;
    [SerializeField] private ItemData product;

    public ItemData Ingredient1 => ingredient1;
    public ItemData Ingredient2 => ingredient2;
    public bool GetsDestroyed1 => getsDestroyed1;
    public bool GetsDestroyed2 => getsDestroyed2;
    public ItemData Product => product;

    public bool IngredientsCompatible(ItemData a, ItemData b)
    {
        return (a.Equals(ingredient1) && b.Equals(ingredient2)) || (a.Equals(ingredient2) && b.Equals(ingredient1));
    }
    
}
