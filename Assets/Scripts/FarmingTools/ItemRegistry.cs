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
        if (_lookup != null ) return;
        _lookup = new Dictionary<string, ItemData>();
        if (allItems == null) return;

        foreach (var item in allItems)
        {
            if (item == null || string.IsNullOrEmpty(item.itemName)) continue;
            _lookup[item.itemName] = item;
        }
    }

    public ItemData GetByID(string id)
    {
        if (_lookup == null) Initialize();
        _lookup.TryGetValue(id, out var result);
        return result;
    }
}
