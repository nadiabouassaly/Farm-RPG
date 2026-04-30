using UnityEngine;

public class InventorySlot
{
    public ItemData item;
    public int quantity;

    public bool IsEmpty()
    {
        return item == null;
    }
    public bool CanAdd(ItemData incoming)
    {
        return item == incoming && quantity < item.maxStackSize;
    }

    public void Clear() { item = null; quantity = 0; }
}
