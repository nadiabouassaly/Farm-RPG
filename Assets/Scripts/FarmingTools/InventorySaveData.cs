using System.Collections.Generic;
// InventorySaveData.cs
[System.Serializable]
public class InventorySlotSaveData
{
    public string itemID;   // References the ScriptableObject by ID
    public int quantity;
}

[System.Serializable]
public class InventorySaveData
{
    public List<InventorySlotSaveData> slots;
}