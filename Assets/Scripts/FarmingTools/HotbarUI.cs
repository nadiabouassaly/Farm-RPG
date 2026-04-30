using System.Collections.Generic;
using UnityEngine;

public class HotbarUI : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private Transform hotbarParent;
    [SerializeField] private Hotbar hotbar;

    private List<InventorySlotUI> slotUIs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slotUIs = new List<InventorySlotUI>();
        for (int i = 0; i < hotbar.hotbarSize; i++)
        {
            GameObject go = Instantiate(slotPrefab, hotbarParent);
            slotUIs.Add(go.GetComponent<InventorySlotUI>());
        }
        Inventory.Instance.OnInventoryChanged += Refresh;
        Refresh();
    }

    // Update is called once per frame
    public void Refresh()
    {
        List<InventorySlot> slots = Inventory.Instance.slots;
        for (int i = 0; i < slotUIs.Count; i++)
        {
            slotUIs[i].Refresh(slots[i]);
            slotUIs[i].SetHighlight(i == hotbar.selectedIndex);
        }
    }
}
