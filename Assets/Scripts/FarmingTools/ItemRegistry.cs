using System.Collections.Generic;
using UnityEngine;

// ItemRegistry.cs
[CreateAssetMenu(fileName = "ItemRegistry", menuName = "Inventory/Item Registry")]
public class ItemRegistry : ScriptableObject
{
    public List<ItemData> allItems;

    private Dictionary<string, ItemData> _lookup;

    public void Initialize()
    {
        _lookup = new Dictionary<string, ItemData>();
        foreach (var item in allItems)
            _lookup[item.itemName] = item;
    }

    public ItemData GetByID(string id)
    {
        _lookup.TryGetValue(id, out var result);
        return result;
    }
}