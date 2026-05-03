using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{   
    public static Inventory Instance { get; private set; }
    [SerializeField] private int inventorySize = 36;
    [SerializeField] public ItemRegistry registry;
    public List<InventorySlot> slots;
    public event System.Action OnInventoryChanged;
    public int Money;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        slots = new List<InventorySlot>();
        for (int i = 0; i < inventorySize; i++)
        {
            slots.Add(new InventorySlot());
        }
        GameEvents.OnLoadDataEvent.AddListener(TriggerLoadPlayerInventory);
    }
    
    public bool AddItem(ItemData item, int quantity = 1)
    {
        if (item == null)
        {
            return false;
        }
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].CanAdd(item))
            {
                int space = item.maxStackSize - slots[i].quantity;
                int toAdd = Mathf.Min(space, quantity);
                slots[i].quantity += toAdd;
                quantity -= toAdd;
                if (quantity == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }
        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].IsEmpty())
            {
                int toAdd = Mathf.Min(item.maxStackSize, quantity);
                slots[i].item = item;
                slots[i].quantity = toAdd;
                quantity -= toAdd;
                if (quantity == 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }
        }
        return false;
    }

    public bool RemoveItem(ItemData item, int quantity = 1)
    {
        if (GetTotalCount(item) < quantity)
        {
            return false;
        }

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i].item == item && !slots[i].IsEmpty())
            {
                int toRemove = Mathf.Min(quantity, slots[i].quantity);
                slots[i].quantity -= toRemove;
                if (slots[i].quantity == 0)
                {
                    slots[i].Clear();
                }
                quantity -= toRemove;
                if (quantity == 0)
                {
                    break;
                }
            }
        }
        OnInventoryChanged?.Invoke();
        return true;
    }

    public int AddItemByIndex(ItemData item, int slotIndex, int quantity = 1)
    {
        int amountLeft = quantity;
        if(slots[slotIndex].IsEmpty())
        {
            slots[slotIndex].item = item;
            slots[slotIndex].quantity = Mathf.Min(quantity, item.maxStackSize);
            amountLeft -= slots[slotIndex].quantity;
            OnInventoryChanged?.Invoke();
        } else if (!slots[slotIndex].IsEmpty() && slots[slotIndex].CanAdd(item))
        {
            int toAdd = Mathf.Min(item.maxStackSize - slots[slotIndex].quantity, quantity);
            slots[slotIndex].quantity += toAdd;
            amountLeft -= toAdd;
            OnInventoryChanged?.Invoke();
        }
        
        return amountLeft;
    }

    public int RemoveItemByIndex(ItemData item, int slotIndex, int quantity = 1)
    {
        if (slots[slotIndex].IsEmpty() || slots[slotIndex].item != item || slots[slotIndex].quantity < quantity)
        {
            return 0;
        }
        int removed = 0;
        int toRemove = Mathf.Min(quantity, slots[slotIndex].quantity);
        slots[slotIndex].quantity -= toRemove;
        removed += toRemove;
        if (slots[slotIndex].quantity == 0)
        {
            slots[slotIndex].Clear();
        }
        OnInventoryChanged?.Invoke();
        return removed;
    }

    public int GetTotalCount(ItemData item)
    {
        int total = 0;
        foreach (var slot in slots)
        {
            if (slot.item == item)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    public int GetItemQuantity(string name) {
        int total = 0;
        foreach (var slot in slots) {
            if (slot.item.name == name)
            {
                total += slot.quantity;
            }
        }
        return total;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void NotifyChanged() => OnInventoryChanged?.Invoke();

    public void TriggerLoadPlayerInventory(TextAsset textFile)
    {
        LoadSaveFiles.SaveFile itemList = JsonUtility.FromJson<LoadSaveFiles.SaveFile>(textFile.text);
        Money = itemList.Money;
        for (int i = 0; i < itemList.invSave.slots.Count; i++)
        {
            var slotData = itemList.invSave.slots[i];
            slots[i].item     = registry.GetByID(slotData.itemID); // Resolves SO reference
            slots[i].quantity = slotData.quantity;
        }
    }

    public void IncreaseMoney(int amount)
    {
        Money += amount;
    }
    public void DecreaseMoney(int amount)
    {
        Money -= amount;
    }

    public void SetMoney(int amount)
    {
        Money = amount;
    }
}
