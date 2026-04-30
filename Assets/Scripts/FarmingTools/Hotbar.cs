using UnityEngine;
using UnityEngine.InputSystem;

public class Hotbar : MonoBehaviour
{
    public int hotbarSize = 9;
    public int selectedIndex = 0;
    public Inventory inventory;
    public ItemData selectedItem;
    public HotbarUI hotbarUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = Inventory.Instance;
        selectedItem = inventory.slots[selectedIndex].item;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            SelectSlot(0);
        }
        if (Keyboard.current.digit2Key.wasPressedThisFrame)
        {
            SelectSlot(1);
        }
        if (Keyboard.current.digit3Key.wasPressedThisFrame)
        {
            SelectSlot(2);
        }
        if (Keyboard.current.digit4Key.wasPressedThisFrame)
        {
            SelectSlot(3);
        }
        if (Keyboard.current.digit5Key.wasPressedThisFrame)
        {
            SelectSlot(4);
        }
        if (Keyboard.current.digit6Key.wasPressedThisFrame)
        {
            SelectSlot(5);
        }
        if (Keyboard.current.digit7Key.wasPressedThisFrame)
        {
            SelectSlot(6);
        }
        if (Keyboard.current.digit8Key.wasPressedThisFrame)
        {
            SelectSlot(7);
        }
        if (Keyboard.current.digit9Key.wasPressedThisFrame)
        {
            SelectSlot(8);
        }
    }

    void SelectSlot(int index)
    {
        if (index < 0 || index >= hotbarSize) return;
        if (selectedIndex == index) return;

        selectedIndex = index;
        ApplySelectedItem();
        UpdateHotbarUI();
    }

    void ApplySelectedItem()
    {
        if (inventory == null || selectedIndex < 0 || selectedIndex >= inventory.slots.Count)
        {
            selectedItem = null;
            return;
        }
        selectedItem = inventory.slots[selectedIndex].item;
    }

    void UpdateHotbarUI()
    {
        hotbarUI.Refresh();
    }
}
