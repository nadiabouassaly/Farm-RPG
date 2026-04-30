using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI quantity;
    [SerializeField] private GameObject highlight;
    [HideInInspector] public int slotIndex;
    [HideInInspector] public InventoryUI inventoryUI;
    private CanvasGroup canvasGroup;
    public static InventorySlotUI currentlyDragging;
    public static GameObject dragIconGO;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Refresh(InventorySlot slot)
    {
        if (slot.IsEmpty())
        {
            itemIcon.sprite = null;
            itemIcon.enabled = false;
            quantity.text = "";
        } else
        {
            itemIcon.sprite = slot.item.itemIcon;
            itemIcon.enabled = true;
            quantity.text = slot.quantity > 1 ? slot.quantity.ToString() : "";
        }
    }

    public void SetHighlight(bool active)
    {
        highlight.SetActive(active);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        var slot = Inventory.Instance.slots[slotIndex];
        if (slot.IsEmpty()) return;

        currentlyDragging = this;

        // Create a floating icon that follows the cursor
        dragIconGO = new GameObject("DragIcon");
        dragIconGO.transform.SetParent(inventoryUI.canvas.transform, false);
        dragIconGO.transform.SetAsLastSibling(); // render on top of everything

        var img = dragIconGO.AddComponent<Image>();
        img.sprite = slot.item.itemIcon;
        img.raycastTarget = false; // IMPORTANT: lets raycasts pass through to slots below
        
        var rect = dragIconGO.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(50, 50);

        // Make the original slot semi-transparent while dragging
        canvasGroup.alpha = 0.4f;
        // Stop the original slot from blocking raycasts while dragging
        // so the slot underneath can receive the OnDrop event
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIconGO == null) return;

        // Move the floating icon to follow the mouse
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            inventoryUI.canvas.GetComponent<RectTransform>(),
            eventData.position,
            inventoryUI.canvas.worldCamera,
            out Vector2 localPoint
        );
        dragIconGO.GetComponent<RectTransform>().localPosition = localPoint;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Clean up whether the drop succeeded or not
        if (dragIconGO != null)
        {
            Destroy(dragIconGO);
            dragIconGO = null;
        }

        // Restore the original slot's appearance
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        currentlyDragging = null;
    }

    public void OnDrop(PointerEventData eventData)
    {
        // This fires on the DESTINATION slot
        if (currentlyDragging == null) return;
        if (currentlyDragging == this) return; // dropped on itself, do nothing

        SwapSlots(currentlyDragging.slotIndex, this.slotIndex);
    }

    private void SwapSlots(int indexA, int indexB)
    {
        var slots = Inventory.Instance.slots;
        (slots[indexA], slots[indexB]) = (slots[indexB], slots[indexA]);
        Inventory.Instance.NotifyChanged();
    }
}
