using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject panel;
    [SerializeField] public Canvas canvas;

    private List<InventorySlotUI> slotUIs = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < Inventory.Instance.slots.Count; i++)
        {
            GameObject go = Instantiate(slotPrefab, gridParent);
            var slotUI = go.GetComponent<InventorySlotUI>();
            slotUI.slotIndex = i;
            slotUI.inventoryUI = this;
            slotUIs.Add(slotUI);
        }

        Inventory.Instance.OnInventoryChanged += Refresh;
        Refresh(); 
        panel.SetActive(false); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            panel.SetActive(!panel.activeSelf);
            FarmingManager.Instance.disabled = !FarmingManager.Instance.disabled;
        }
    }

    void Refresh()
    {
        List<InventorySlot> slots = Inventory.Instance.slots;
        for (int i = 0; i < slotUIs.Count; i++)
            slotUIs[i].Refresh(slots[i]);
    }
}
